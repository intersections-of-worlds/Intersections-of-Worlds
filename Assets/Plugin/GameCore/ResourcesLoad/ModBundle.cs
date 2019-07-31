using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Entities;
using System.Reflection;

namespace GameCore
{

    public abstract class ModBundle
    {
        protected AssetBundle ab;
        /// <param name="path">不带后缀的Mod路径</param>
        public ModBundle(ModInfo info, string path, SaveManager save)
        {
            ModPath = path;
            Info = info;
            Isloaded = false;
            Save = save;
        }

        /// <summary>
        /// Mod基本信息
        /// </summary>
        public ModInfo Info { get; private set; }
        /// <summary>
        /// Mod的路径，不带后缀
        /// </summary>
        public string ModPath { get; private set; }
        /// <summary>
        /// 是否已被加载
        /// </summary>
        public bool Isloaded { get; private set; }
        /// <summary>
        /// Mod所属存档
        /// </summary>
        public SaveManager Save { get; private set; }
        /// <summary>
        /// 从Mod中加载资源
        /// </summary>
        /// <typeparam name="T">资源的类型</typeparam>
        public abstract T Get<T>(string AssetName);
        /// <summary>
        /// 加载该Mod内所有该类型的资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public abstract T[] GetAll<T>();
        /// <summary>
        /// 将Mod内容加载到内存中
        /// </summary>
        public virtual void Load()
        {
            var path = ModPath;
#if UNITY_IOS //IOS下包处理逻辑
            path+=".iosmod";
#elif UNITY_ANDROID //安卓下包处理逻辑
            path+=".androidmod";
#elif UNITY_STANDALONE_LINUX //Linux下包处理逻辑
            path+=".linuxmod";
#elif UNITY_STANDALONE_WIN //Windows下包处理逻辑，经测试，windows版editor运行时，也有UNITY_STANDALONE_WIN标签
            path += ".winmod";
#elif UNITY_STANDALONE_OSX //OSX下包处理逻辑
            path += ".osxmod
#endif
            ab = AssetBundle.LoadFromFile(path);
            Init();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {
            //给ab内所有资源设置索引
        }

        public abstract ComponentSystemGroup GetUpdateSystemGroup();
        /// <summary>
        /// 获得该Mod的所有依赖Mod
        /// </summary>
        public ModMatcher[] GetDependences()
        {
            return Info.dependences;
        }
        /// <summary>
        /// 获得指定路径的ModBundle
        /// </summary>
        /// <param name="info">Mod信息</param>
        /// <param name="path">Mod路径（不带后缀）</param>
        public static ModBundle Creat(ModInfo info, string path,SaveManager save)
        {
            switch (info.bundleType)
            {
                case BundleType.CSBundle:
                    return new CSBundle(info, path,save);
            }
            throw new ArgumentNullException();
        }
    }
    public enum BundleType
    {
        CSBundle
    }
    public class CSBundle : ModBundle
    {
        public CSBundle(ModInfo info, string path,SaveManager save) : base(info, path,save)
        {

        }

        public override T Get<T>(string AssetName)
        {
            throw new NotImplementedException();
        }

        public override T[] GetAll<T>()
        {
            throw new NotImplementedException();
        }
        public override ComponentSystemGroup GetUpdateSystemGroup()
        {
            //如当前代码无法使用则使用下方代码

            //如果找系统组失败就抛出错误（各种原因，如该名称不存在，该类未继承ComponentSystemGroup之类的）
            try
            {
                Type t = Type.GetType(Info.InternalName + "." + Info.InternalName + "UpdateGroup");
                return (ComponentSystemGroup)Activator.CreateInstance(t);
            }
            catch (Exception)
            {
                throw new SystemGroupGetError("UpdateGroup");
            }
            /*
            //在程序集中找到对应系统组的类（内部名+UpdateGroup）返回
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for(int i = 0; i < assemblies.Length; i++)
            {
                
                if(assemblies[i].GetName().Name == Info.InternalName)
                {
                    //如果找系统组失败就抛出错误（各种原因，如该名称不存在，该类未继承ComponentSystemGroup之类的）
                    try
                    {
                        return (ComponentSystemGroup)assemblies[i].CreateInstance(Info.InternalName + "." + Info.InternalName + "UpdateGroup");
                    }
                    catch (Exception)
                    {
                        throw new SystemGroupGetError("UpdateGroup");
                    }
                }
            }
            //没找到程序集就抛出错误
            throw new ModAssemblyIsNotExistError(Info.InternalName);*/

        }

        public override void Load()
        {


            base.Load();
        }
        public class ModAssemblyIsNotExistError : Exception
        {
            public string ModInternalName;
            public ModAssemblyIsNotExistError(string modInternalName) : base("未能找到Mod " + modInternalName + "的程序集")
            {
                ModInternalName = modInternalName;
            }
        }
        public class SystemGroupGetError : Exception
        {
            public string GroupTypeName;
            public SystemGroupGetError(string groupTypeName) : base("未能成功获取到" + groupTypeName)
            {
                GroupTypeName = groupTypeName;
            }
        }
    }
    /// <summary>
    /// 用于匹配Mod
    /// </summary>
    [Serializable]
    public struct ModMatcher : IEquatable<ModMatcher>
    {
        /// <summary>
        /// Mod名称
        /// </summary>
        public string ModName;
        /// <summary>
        /// Mod最低版本
        /// </summary>
        public Version oldestVersion;
        /// <summary>
        /// Mod最高版本
        /// </summary>
        public Version newestVersion;

        public ModMatcher(string modName, Version oldestVersion, Version newestVersion)
        {
            if(oldestVersion > newestVersion)
            {
                throw new ArgumentException("最低版本不得大于最高版本！");
            }
            ModName = modName;
            this.oldestVersion = oldestVersion;
            this.newestVersion = newestVersion;
        }

        /// <summary>
        /// 检测一个Mod是否满足依赖条件
        /// </summary>
        /// <param name="name">该Mod名称</param>
        /// <param name="ver">该Mod版本</param>
        public bool IsMatched(string name,Version ver)
        {
            return name == ModName && ver >= oldestVersion && ver <= newestVersion;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is ModMatcher))
            {
                return false;
            }

            var info = (ModMatcher)obj;
            return this == info;
        }

        public bool Equals(ModMatcher other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            var hashCode = -820004177;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ModName);
            hashCode = hashCode * -1521134295 + EqualityComparer<Version>.Default.GetHashCode(oldestVersion);
            hashCode = hashCode * -1521134295 + EqualityComparer<Version>.Default.GetHashCode(newestVersion);
            return hashCode;
        }

        public static bool operator==(ModMatcher a,ModMatcher b)
        {
            return a.ModName == b.ModName && a.oldestVersion == b.oldestVersion && a.newestVersion == b.newestVersion;
        }
        public static bool operator !=(ModMatcher a, ModMatcher b)
        {
            return !(a == b);
        }
    }
}
