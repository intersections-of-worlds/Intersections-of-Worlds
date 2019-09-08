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
        public AssetIndexer Indexer { get; private set; }
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
        public virtual T Get<T>(string AssetFullName) where T : UnityEngine.Object
        {
            return ab.LoadAsset<T>(AssetFullName);
        }
        /// <summary>
        /// 从Mod中加载资源
        /// </summary>
        public virtual T Get<T>(int AssetId) where T : UnityEngine.Object
        {
            int index = Indexer.TryGet(AssetId);
            if (index == -1)
                return null;
            else
                return ab.LoadAsset<T>(Indexer[index].FullName);
        }
        /// <summary>
        /// 获得Mod中资源的引用
        /// </summary>
        public virtual AssetRef GetRef(string AssetFullName)
        {
            return Indexer[AssetFullName].GetRef();
        }
        /// <summary>
        /// 加载该Mod内所有该类型的资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        public virtual T[] GetAll<T>() where T : UnityEngine.Object
        {
            return ab.LoadAllAssets<T>();
        }
        /// <summary>
        /// 获得Mod中所有该类型资源的引用
        /// </summary>
        public virtual List<AssetRef> GetAllRef<T>() where T : UnityEngine.Object
        {
            List<AssetRef> result = new List<AssetRef>();
            for(int i = 0; i < Indexer.Count; i++)
            {
                if (Indexer[i].Is<T>())
                {
                    result.Add(Indexer[i].GetRef());
                }
            }
            return result;
        }
        /// <summary>
        /// 加载该Mod内所有匹配这些Tag的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="All">资源必须包含的Tag</param>
        /// <param name="None">资源不应包含的Tag</param>
        public virtual List<T> GetAllByTags<T>(string[] All, string[] None) where T : UnityEngine.Object
        {
            List<T> result = new List<T>();
            for (int i = 0; i < Indexer.Count; i++)
            {
                if (Indexer[i].IsMatchTag(All, None) && Indexer[i].Is(typeof(T)))
                    result.Add(ab.LoadAsset<T>(Indexer[i].FullName));
            }
            return result;
        }
        /// <summary>
        /// 获得该Mod内所有匹配这些Tag的资源的引用
        /// </summary>
        public virtual List<AssetRef> GetAllRefByTags<T>(string[] All, string[] None) where T : UnityEngine.Object
        {
            List<AssetRef> result = new List<AssetRef>();
            for (int i = 0; i < Indexer.Count; i++)
            {
                if (Indexer[i].IsMatchTag(All, None) && Indexer[i].Is(typeof(T)))
                    result.Add(Indexer[i].GetRef());
            }
            return result;
        }
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
            //GameString.AddDic(ab.LoadAsset<LanguageDictionary>("LanguageDic"));
            Indexer = ab.LoadAsset<AssetIndexer>("AssetIndexer");
        }
        public virtual void Unload()
        {
            ab.Unload(true);
        }
        public virtual AssetInfo GetAssetInfo(string AssetFullName)
        {
            return Indexer[AssetFullName];
        }
        public abstract ComponentSystemGroup GetUpdateSystemGroup();
        public abstract SerializationSystemGroup GetSerializationSystemGroup();
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

        public override SerializationSystemGroup GetSerializationSystemGroup()
        {
            //在程序集中找到对应系统组的类（内部名+SerializationGroup）返回
            try
            {
                return (SerializationSystemGroup)World.Active.CreateSystem(Assembly.GetExecutingAssembly().
                    GetType(Info.InternalName + "." + Info.InternalName + "SerializationGroup"));
            }
            catch (Exception e)
            {
                throw new SystemGroupGetError("Serialization", e);
            }
        }

        public override ComponentSystemGroup GetUpdateSystemGroup()
        {
            //在程序集中找到对应系统组的类（内部名+UpdateGroup）返回
            try
            {
                return (ComponentSystemGroup)World.Active.CreateSystem(Assembly.GetExecutingAssembly().
                    GetType(Info.InternalName + "." + Info.InternalName + "UpdateGroup"));
            }
            catch (Exception e)
            {
                throw new SystemGroupGetError("UpdateGroup",e);
            }
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
            public SystemGroupGetError(string groupTypeName,Exception innerException) : base("未能成功获取到" + groupTypeName,innerException)
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
    public class ModAssetException : Exception
    {
        public string ModName;
        public string AssetName;

        public ModAssetException(string message,string modname,string assetname) :base(message)
        {
            ModName = modname;
            AssetName = assetname;
        }
    }
}
