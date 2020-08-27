using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameCore
{
    public struct AssetRef
    {
        public Guid ModId;
        public Guid AssetId;

        public string GetModId(){
            return ModId.ToString();
        }
        public string GetAssetId(){
            return AssetId.ToString();
        }
        public void SetModId(string modId){
            ModId = Guid.Parse(modId);
        }
        public void SetAssetId(string assetId){
            AssetId = Guid.Parse(assetId);
        }

        public AssetRef(Guid modId, Guid assetId)
        {
            ModId = modId;
            AssetId = assetId;
        }
        public AssetRef(string modId, string assetId){
            SetModId(modId);
            SetAssetId(assetId);
        }
        public T Get<T>() where T : UnityEngine.Object
        {
            if (ModId.Equals(Guid.Empty) && AssetId.Equals(Guid.Empty))//处理空引用
                return null;
            return SaveManager.Active.Get<T>(this);
        }
        public string GetAssetFullName()
        {
            if (ModId.Equals(Guid.Empty) && AssetId.Equals(Guid.Empty))//处理空引用
                return "None";
            return SaveManager.Active.GetModById(ModId.ToString()).AssetManager.TryGetAssetFullName(AssetId.ToString());
        }
    }

}

