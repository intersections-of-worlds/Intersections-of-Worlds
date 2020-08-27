using System;

namespace GameCore
{
    /// <summary>
    /// 版本类，注：如果某一版本值为最低，请使用-1，如果某一版本值为最高，请使用255，如1.1.-1和1.1.255
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

        public readonly static Version lowestVersion = new Version(-1, -1, -1);
        public readonly static Version highestVersion = new Version(255, 255, 255);
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
