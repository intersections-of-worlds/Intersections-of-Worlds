using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public struct AssetRef
    {
        public int ModId;
        public int AssetId;

        public AssetRef(int modId, int assetId)
        {
            ModId = modId;
            AssetId = assetId;
        }

        public T Get<T>() where T : UnityEngine.Object
        {
            if (ModId == 0 && AssetId == 0)//处理空引用
                return null;
            return SaveManager.Active.Get<T>(this);
        }
        public string GetAssetFullName()
        {
            if (ModId == 0 && AssetId == 0)//处理空引用
                return "None";
            return SaveManager.Active.GetModById(ModId).AssetManager.TryGetAssetFullName(AssetId);
        }
    }

}

