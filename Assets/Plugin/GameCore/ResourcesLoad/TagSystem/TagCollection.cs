using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameCore
{
    /// <summary>
    /// Tag的集合
    /// </summary>
    [Serializable]
    public class TagCollection : List<string> ,IEquatable<TagCollection>
    {
        public override bool Equals(object obj)
        {
            return Equals((TagCollection)obj);
        }
        public bool Equals(TagCollection other)
        {
            if(Count == other.Count)
            {
                for (int i = 0; i < Count; i++)
                {
                    if(this[i] != other[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        new public void Add(string Tag)
        {
            //如果该tag已添加直接忽视
            if (Contains(Tag))
            {
                return;
            }
            if (!Tag.Contains("."))
            {
                throw new ArgumentException("该Tag名称不正确！不含所属Mod名！");
            }
            base.Add(Tag);
        }
        public void Add(string ModName,string TagName)
        {
            Add(ModName + "." + TagName);
        }
        public override int GetHashCode()
        {
            uint seed = 13131;
            uint hash = 0;
            for (int i = 0; i < Count; i++)
            {
                hash = hash * seed + (uint)this[i].GetHashCode();
            }
            return (int)(hash & 0x7FFFFFFF);
        }
    }
}
