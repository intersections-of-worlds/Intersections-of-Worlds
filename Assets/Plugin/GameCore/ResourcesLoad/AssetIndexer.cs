using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameCore
{
    [Serializable]
    public class AssetIndexer : ScriptableObject
    {
        /// <summary>
        /// 资源名和资源信息的对应表
        /// </summary>
        public Dictionary<string,AssetInfo> InfoDic { get; private set; }
        public void Add(string name,AssetInfo asset)
        {
            InfoDic.Add(name,asset);
        }
        public void Remove(string path)
        {
            InfoDic.Remove(name);
        }
        public AssetIndexer()
        {
            InfoDic = new Dictionary<string, AssetInfo>();
        }
    }
}
