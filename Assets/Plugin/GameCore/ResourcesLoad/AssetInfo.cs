using System;
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
    }
}
