using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
namespace GameCore {
    /// <summary>
    /// 整个存档的管理类
    /// </summary>
    public class SaveManager
    {
        public SaveInfo Info { get; private set; }
        public List<ModBundle> mods { get; private set; }
        public SaveManager(SaveInfo _info)
        {
            Info = _info;
            mods = new List<ModBundle>();
            for(int i = 0; i < Info.Mods.Length; i++)
            {
                AddMod(Info.Mods[i]);
            }
        }
        #region ModControl
        /// <summary>
        /// 通过包名搜索Mod添加到存档中
        /// </summary>
        /// <param name="name">Mod包名（指文件名，不含后缀）</param>
        public void AddMod(string name)
        {
            var path = FindMod(name);
            if (name == null)
                throw new ArgumentException("该Mod不存在");
            string[] paths = SortDependences(GetDependencesTable(path));
            for(int i = 0; i < paths.Length; i++)
            {
                mods.Add(ModBundle.Creat(LoadModInfo(paths[i]), paths[i]));
            }
        }
        /// <summary>
        /// 检查该存档内有没有该Mod
        /// </summary>
        /// <param name="ModPath">Mod的路径，不带后缀</param>
        public bool Contains(string ModPath)
        {
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].ModPath == ModPath)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 通过Mod包名找到Mod路径，找不到返回null
        /// </summary>
        /// <param name="name">Mod包名</param>
        /// <returns>Mod路径，加个后缀即可使用</returns>
        private string FindMod(string name)
        {
            //先搜索存档中是否有此Mod包
            string path = Application.persistentDataPath + "/Saves/" + Info.Name + "/Mods/" + name;
            if (Directory.Exists(path) && CheckMod(name, path))
                return path + "/" + name;//帮忙把第二层打上，加载只需接后缀即可
            //再从玩家Mod列表里找
            path = Application.persistentDataPath + "/Mods/" + name;
            if (Directory.Exists(path) && CheckMod(name, path))
                return path + "/" + name;
            //最后在游戏自带Mod列表里找
            path = Application.streamingAssetsPath + "/Mods/" + name;
            if (Directory.Exists(path))//游戏自带Mod不需要检查其完整性
                return path + "/" + name;
            return null;
        }
        /// <summary>
        /// 检测一个Mod包是否完整
        /// </summary>
        /// <param name="name">Mod包包名
        /// <param name="path">Mod包路径</param>
        /// <returns></returns>
        private static bool CheckMod(string name,string path)
        {
            //检查五个平台的assetbundle和Mod基本信息是否都存在
            return File.Exists(path + "/" + name + ".iosmod") && File.Exists(path + "/" + name + ".winmod")
                && File.Exists(path + "/" + name + ".androidmod") && File.Exists(path + "/" + name + ".osxmod")
                && File.Exists(path + "/" + name + ".linuxmod") && File.Exists(path + "/" + name + ".json");
        }
        /// <summary>
        /// 将包名解析为Mod名和版本
        /// </summary>
        /// <param name="packagename">包名</param>
        /// <param name="ver">Mod版本</param>
        /// <returns>Mod名</returns>
        private static string GetModInfoByName(string packagename,out Version ver)
        {
            //如果包名中带了后缀，抛出错误
            if (packagename.Contains("."))
                throw new ArgumentException("Mod包名不应携带后缀！！！");
            //将包名由空格分隔开
            var s = packagename.Split(' ');
            //如果包名中不止一个空格，报错
            if(s.Length != 2)
            {
                throw new ArgumentException("Mod包名有误！");
            }
            //名称在前，版本在后
            ver = new Version(s[1]);
            return s[0];
        }
        /// <summary>
        /// 加载Mod的信息
        /// </summary>
        /// <param name="path">Mod路径（不带后缀！）</param>
        private static ModInfo LoadModInfo(string path)
        {
            //加上后缀
            path += ".json";
            return JsonConvert.DeserializeObject<ModInfo>(File.ReadAllText(path));
            
        }
        /// <summary>
        /// 获得一个Mod的依赖表（方便使用版）
        /// </summary>
        /// <param name="path">该Mod的路径</param>
        /// <returns>依赖表（Key为Mod的路径，Value为Mod所依赖的Mod的路径）</returns>
        private Dictionary<string,List<string>> GetDependencesTable(string path)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            GetDependencesTable(path, ref result); ;
            return result;
        }
        /// <summary>
        /// 获得一个Mod的依赖表（实现版）
        /// </summary>
        private void GetDependencesTable(string path,ref Dictionary<string, List<string>> result)
        {
            //防止重复找同一Mod的依赖
            if (result.ContainsKey(path))
                return;
            //Mod的依赖Mod路径list，如果没有依赖就是个空list
            var list = new List<string>();
            result.Add(path, list);
            //通过递归一层层找出Mod依赖关系
            var infos = LoadModInfo(path).dependences;
            //先找Mod
            for (int i = 0; i < infos.Length; i++)
            {
                //找到依赖的Mod后，如果该Mod不在存档中，就将其添加到该Mod依赖列表（在就不用添加了，因为已在加载队列，就可以丢掉不管）
                var demod = MatchMod(infos[i]);
                if (demod == null)
                    throw new ModDependenceException("找不到依赖项！",GetModPackageNameByPath(path),infos[i]);
                if (Contains(demod))
                    continue;
                list.Add(demod);
                //并继续找它的依赖
                GetDependencesTable(demod,ref result);
            }
        }
        /// <summary>
        /// 在已有Mod中通过依赖信息匹配合适的Mod
        /// </summary>
        /// <param name="info">依赖信息</param>
        /// <returns>匹配到的Mod的路径，没匹配到返回null</returns>
        private string MatchMod(ModDependencesInfo info)
        {
            //先看看存档里有没有此Mod，有就直接返回，省去磁盘读取的时间
            var result = MatchModinSave(info);
            if (result != null)
                return result;
            //在存档内Mod列表匹配
            string path = Application.persistentDataPath + "/Saves/" + Info.Name + "/Mods";
            DirectoryInfo[] ds = new DirectoryInfo(path).GetDirectories();
            for(int i = 0; i < ds.Length; i++)
            {
                Version v;
                //分解包名信息
                string name = GetModInfoByName(ds[i].Name,out v);
                //检测是否匹配
                if (info.IsMatched(name, v))
                    return ds[i].FullName;
            }


            //然后在玩家Mod列表匹配
            path = Application.persistentDataPath + "/Mods";
            ds = new DirectoryInfo(path).GetDirectories();
            for (int i = 0; i < ds.Length; i++)
            {
                Version v;
                //分解包名信息
                string name = GetModInfoByName(ds[i].Name, out v);
                //检测是否匹配
                if (info.IsMatched(name, v))
                    return ds[i].FullName;
            }


            //最后在自带Mod中匹配
            path = Application.streamingAssetsPath + "/Mods";
            ds = new DirectoryInfo(path).GetDirectories();
            for (int i = 0; i < ds.Length; i++)
            {
                Version v;
                //分解包名信息
                string name = GetModInfoByName(ds[i].Name, out v);
                //检测是否匹配
                if (info.IsMatched(name, v))
                    return ds[i].FullName;
            }
            return null;
        }
        /// <summary>
        /// 在存档内通过依赖信息匹配Mod
        /// </summary>
        /// <param name="info">依赖信息</param>
        /// <returns>匹配到的Mod的路径，没匹配到返回null</returns>
        private string MatchModinSave(ModDependencesInfo info)
        {
            for (int i = 0; i < mods.Count; i++)
            {
                Version v;
                //分解包名信息
                string name = GetModInfoByName(mods[i].ModPath, out v);
                //检测是否匹配
                if (info.IsMatched(name, v))
                    return mods[i].ModPath;
            }
            return null;
        }
        /// <summary>
        /// 将依赖表中的Mod按依赖排序成加载顺序
        /// </summary>
        /// <param name="dic">依赖表</param>
        /// <returns>Mod加载顺序</returns>
        private static string[] SortDependences(Dictionary<string,List<string>> dic)
        {
            int lastLength = 0;
            List<string> result = new List<string>();
            while(dic.Count != 0)
            {
                //如果经过一轮处理dic长度没有变化，就出现了循环依赖，抛出错误
                if (lastLength == dic.Count)
                    throw new ModDependenceException("Mod出现了循环依赖！！");
                lastLength = dic.Count;
                //检测有没有依赖已被处理完成的Mod，有就将其添加进列表并删除别的Mod的依赖
                foreach(var pair in dic)
                {
                    if(pair.Value.Count == 0)
                    {
                        string key = pair.Key;
                        result.Add(key);
                        dic.Remove(key);
                        //从dic中删除别的Mod的对应依赖
                        foreach(var list in dic.Values)
                        {
                            if (list.Contains(key))
                                list.Remove(key);
                        }

                    }
                }
            }
            return result.ToArray();
        }
        /// <summary>
        /// 通过Mod路径获得Mod包名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetModPackageNameByPath(string path)
        {
            var s = path.Split('/');
            return s[s.Length - 1];
        }
        #endregion
        #region Runtime
        private void Load()
        {

        }
        #endregion

    }
    /// <summary>
    /// 储存存档信息的类
    /// </summary>
    [Serializable]
    public class SaveInfo
    {
        /// <summary>
        /// 存档名
        /// </summary>
        public string Name;
        /// <summary>
        ///该存档拥有的 Mod的包名列表
        /// </summary>
        public string[] Mods;
    }
    public class ModDependenceException : Exception
    {
        public string ModPackageName;
        public ModDependencesInfo DependenceMod;

        public ModDependenceException(string message) : base(message)
        {
        }

        public ModDependenceException(string message,string modPackageName,ModDependencesInfo dependenceMod) : base(message)
        {
            ModPackageName = modPackageName;
            DependenceMod = dependenceMod;
        }
    }

}
