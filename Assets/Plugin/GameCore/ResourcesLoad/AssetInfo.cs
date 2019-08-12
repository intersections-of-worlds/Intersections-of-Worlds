using System;
using System.Collections.Generic;
using UnityEngine;
namespace GameCore
{
    [Serializable]
    public partial class AssetInfo
    {
        public string ModName;
        public string AssetName;
        public int ModId;
        public int AssetId;
        public string AssetGuid;
        public string TypeName;
        public string FullName { get { return ModName + "." + AssetName; } }
        public string[] BaseTypes;
        /// <summary>
        /// 该资源的Tag的集合
        /// </summary>
        public TagCollection Tags;
        public AssetInfo(string modName,string assetName,int modId,string assetGuid,Type t)
        {
            ModName = modName;
            AssetName = assetName;
            ModId = modId;
            AssetId = new Guid(assetGuid).GetHashCode();
            TypeName = t.FullName;
            AssetGuid = assetGuid;
            string tn;
            List<string> baseTypes = new List<string>();
            do
            {
                t = t.BaseType;
                tn = t.FullName;
                baseTypes.Add(tn);
            } while (!t.Equals(typeof(UnityEngine.Object)));
            BaseTypes = baseTypes.ToArray();
        }
        /// <summary>
        /// 检测资源是否是该类型
        /// </summary>
        public bool Is(string TypeFullName)
        {
            if (TypeFullName == TypeName)
                return true;
            for (int i = 0; i < BaseTypes.Length; i++)
            {
                if (TypeFullName == BaseTypes[i])
                    return true;
            }
            return false;
        }
        public bool Is(Type t)
        {
            return Is(t.FullName);
        }
        /// <summary>
        /// 检测该资源是否匹配这些Tag
        /// </summary>
        /// <param name="All">必须包含的tag</param>
        /// <param name="None">不能包含的tag</param>
        /// <returns></returns>
        public bool IsMatchTag(string[] All,string[] None)
        {
            for(int i = 0; i < All.Length; i++)
            {
                if (!Tags.Contains(All[i]))
                    return false;
            }
            if (None != null)
            {
                for (int i = 0; i < None.Length; i++)
                {
                    if (Tags.Contains(None[i]))
                        return false;
                }
            }
            return true;
        }
        public override bool Equals(object obj)
        {
            var info = obj as AssetInfo;
            return info != null &&
                   ModName == info.ModName &&
                   AssetName == info.AssetName &&
                   ModId == info.ModId &&
                   AssetId == info.AssetId &&
                   TypeName == info.TypeName &&
                   EqualityComparer<TagCollection>.Default.Equals(Tags, info.Tags);
        }

        public override int GetHashCode()
        {
            var hashCode = 991503834;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ModName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AssetName);
            hashCode = hashCode * -1521134295 + ModId.GetHashCode();
            hashCode = hashCode * -1521134295 + AssetId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TypeName);
            hashCode = hashCode * -1521134295 + EqualityComparer<TagCollection>.Default.GetHashCode(Tags);
            return hashCode;
        }
    }
}
