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
        public string TypeName;
        /// <summary>
        /// 该资源的Tag的集合
        /// </summary>
        public TagCollection Tags;
        public AssetInfo(string modName,string assetName,int modId,int assetId,string typeName)
        {
            ModName = modName;
            AssetName = assetName;
            ModId = modId;
            AssetId = assetId;
            TypeName = typeName;
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
