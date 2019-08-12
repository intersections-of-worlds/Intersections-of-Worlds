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
    public class TagCollection : IList<string>, IEquatable<TagCollection>
    {
        public int Count { get { return content.Count; } }

        public bool IsReadOnly => ((IList<string>)content).IsReadOnly;

        string IList<string>.this[int index] { get => ((IList<string>)content)[index]; set => ((IList<string>)content)[index] = value; }

        public List<string> content = new List<string>();
        public string this[int index]
        {
            get { return content[index]; }
        }
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
                    if(content[i] != other[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public void Add(string Tag)
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
            content.Add(Tag);
        }
        public bool Contains(string Tag)
        {
            return content.Contains(Tag);
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

        public int IndexOf(string item)
        {
            return ((IList<string>)content).IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            ((IList<string>)content).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<string>)content).RemoveAt(index);
        }

        public void Clear()
        {
            ((IList<string>)content).Clear();
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            ((IList<string>)content).CopyTo(array, arrayIndex);
        }

        public bool Remove(string item)
        {
            return ((IList<string>)content).Remove(item);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ((IList<string>)content).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<string>)content).GetEnumerator();
        }
    }
}
