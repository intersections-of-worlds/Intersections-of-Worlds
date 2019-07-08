using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Entities;

namespace GameCore
{

    public abstract class ModBundle
    {
        public ModInfo info;
    }
    /// <summary>
    /// Mod的信息
    /// </summary>
    [CreateAssetMenu(menuName ="Intersection of Worlds/ModInfo")]
    [Serializable]
    public class ModInfo : ScriptableObject
    {
        /// <summary>
        /// Mod名称
        /// </summary>
        public string ModName;
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
    }
    /// <summary>
    /// Mod依赖的Mod的信息
    /// </summary>
    [Serializable]
    public struct ModDependencesInfo
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
                throw new ArgumentException();
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
