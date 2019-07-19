using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Entities;

namespace GameCore
{

    public abstract class ModBundle
    {
        /// <param name="path">不带后缀的Mod路径</param>
        public ModBundle(ModInfo info,string path,SaveManager save)
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
        public abstract T Get<T>();
        /// <summary>
        /// 加载该Mod内所有该类型的资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public abstract T[] GetAll<T>();
        /// <summary>
        /// 将Mod内容加载到内存中
        /// </summary>
        public abstract void Load();
        /// <summary>
        /// 获得该Mod的所有依赖Mod
        /// </summary>
        public ModDependencesInfo[] GetDependences()
        {
            return Info.dependences;
        }
        /// <summary>
        /// 获得指定路径的ModBundle
        /// </summary>
        /// <param name="info">Mod信息</param>
        /// <param name="path">Mod路径（不带后缀）</param>
        public static ModBundle Creat(ModInfo info, string path)
        {
            switch (info.bundleType)
            {
                case BundleType.CSBundle:
                    return new CSBundle(info, path);
            }
            throw new ArgumentNullException();
        }
        /// <summary>
        /// 初始化Mod内各种内容
        /// </summary>
        public abstract void Init();
    }
    public enum BundleType
    {
        CSBundle
    }
    public class CSBundle : ModBundle
    {
        private AssetBundle ab;
        public CSBundle(ModInfo info, string path,SaveManager save) : base(info, path,save)
        {

        }

        public override T Get<T>()
        {
            throw new NotImplementedException();
        }

        public override T[] GetAll<T>()
        {
            throw new NotImplementedException();
        }

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Load()
        {
            var path = ModPath;


        }
    }
    /// <summary>
    /// Mod的信息
    /// </summary>
    [CreateAssetMenu(menuName ="Intersection of Worlds/ModInfo")]
    [Serializable]
    public class ModInfo : ScriptableObject
    {
        /// <summary>
        /// Mod显示在外的名称
        /// </summary>
        public string ModName;
        /// <summary>
        /// Mod的内部名称，代码中的命名空间。Mod包的名字为 内部名 + 空格 + 版本 ，内部名中请不要有空格
        /// Mod内三个系统组的名字为内部名+SimulationSystemGroup/InitializationSystemGroup/PresentationSystemGroup
        /// </summary>
        public string InternalName;
        /// <summary>
        /// Mod版本
        /// </summary>
        public Version version;
        /// <summary>
        /// Mod作者
        /// </summary>
        public string author;
        /// <summary>
        /// Mod的依赖Mod列表
        /// </summary>
        public ModDependencesInfo[] dependences;
        /// <summary>
        /// Mod描述
        /// </summary>
        public string description;
        /// <summary>
        /// Mod包的类型
        /// </summary>
        public BundleType bundleType;
    }
    /// <summary>
    /// Mod依赖的Mod的信息
    /// </summary>
    [Serializable]
    public struct ModDependencesInfo : IEquatable<ModDependencesInfo>
    {
        /// <summary>
        /// 依赖的Mod名称
        /// </summary>
        public string ModName;
        /// <summary>
        /// 依赖的Mod能兼容的最低版本
        /// </summary>
        public Version oldestVersion;
        /// <summary>
        /// 依赖的Mod能兼容的最高版本
        /// </summary>
        public Version newestVersion;
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
            if (!(obj is ModDependencesInfo))
            {
                return false;
            }

            var info = (ModDependencesInfo)obj;
            return this == info;
        }

        public bool Equals(ModDependencesInfo other)
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

        public static bool operator==(ModDependencesInfo a,ModDependencesInfo b)
        {
            return a.ModName == b.ModName && a.oldestVersion == b.oldestVersion && a.newestVersion == b.newestVersion;
        }
        public static bool operator !=(ModDependencesInfo a, ModDependencesInfo b)
        {
            return !(a == b);
        }
    }
    /// <summary>
    /// 版本类
    /// </summary>
    [Serializable]
    public struct Version : IEquatable<Version>
    {
        /// <summary>
        /// 主版本(x.0.0)
        /// </summary>
        public int Major;
        /// <summary>
        /// 子版本(0.x.0)
        /// </summary>
        public int Minor;
        /// <summary>
        /// 修订版本(0.0.x)
        /// </summary>
        public int Revision;

        public Version(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }
        public Version(string version)
        {
            string[] s = version.Split('.');
            if(s.Length != 3)
            {
                throw new ArgumentException("版本文字有误！");
            }
            Major = int.Parse(s[0]);
            Minor = int.Parse(s[1]);
            Revision = int.Parse(s[2]);
        }
        public static bool operator<(Version a,Version b)
        {
            if (a.Major < b.Major)
            {
                return true;
            }
            if (a.Major > b.Major)
            {
                return false;
            }
            if(a.Minor < b.Minor)
            {
                return true;
            }
            if (a.Minor > b.Minor)
            {
                return false;
            }
            if(a.Revision < b.Revision)
            {
                return true;
            }
            return false;
        }
        public static bool operator >(Version a, Version b)
        {
            if (a.Major > b.Major)
            {
                return true;
            }
            if (a.Major < b.Major)
            {
                return false;
            }
            if (a.Minor > b.Minor)
            {
                return true;
            }
            if (a.Minor < b.Minor)
            {
                return false;
            }
            if (a.Revision > b.Revision)
            {
                return true;
            }
            return false;
        }
        public static bool operator <=(Version a, Version b)
        {
            return !(a > b);
        }
        public static bool operator >=(Version a, Version b)
        {
            return !(a < b);
        }
        public static bool operator ==(Version a, Version b)
        {
            return a.Major == b.Major && a.Minor == b.Minor && a.Revision == b.Revision;
        }
        public static bool operator !=(Version a, Version b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return Equals((Version)obj);
        }
        public bool Equals(Version other)
        {
            return this == other;
        }
        public override int GetHashCode()
        {
            var hashCode = -327234472;
            hashCode = hashCode * -1521134295 + Major.GetHashCode();
            hashCode = hashCode * -1521134295 + Minor.GetHashCode();
            hashCode = hashCode * -1521134295 + Revision.GetHashCode();
            return hashCode;
        }
        public override string ToString()
        {
            return Major.ToString() + "." + Minor.ToString() + "." + Revision.ToString();
        }
    }
}
