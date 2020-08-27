using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameCore
{
    /// <summary>
    /// 资源信息的索引器，运行时版
    /// </summary>
    [Serializable]
    public partial class AssetIndexer : ScriptableObject
    {
        /// <summary>
        /// 资源名称列表
        /// </summary>
        public List<string> NameList = new List<string>();
        /// <summary>
        /// 资源信息列表
        /// </summary>
        public List<AssetInfo> InfoList = new List<AssetInfo>();
        /// <summary>
        /// 资源总数
        /// </summary>
        /// <value></value>
        public int Count { get { return InfoList.Count; } }
        public void Init()
        {
            if (NameList == null)
            {
                NameList = new List<string>();
                InfoList = new List<AssetInfo>();
            }
        }
        public AssetInfo this[string index]
        {
            get
            {
                return InfoList[NameList.IndexOf(index)];
            }
        }
        public string this[AssetInfo index]
        {
            get {
                return NameList[InfoList.IndexOf(index)];
            }
        }
        public AssetInfo this[int index]
        {
            get
            {
                return InfoList[index];
            }
        }
        /// <summary>
        /// 尝试通过AssetId获得资源，没成功返回-1
        /// </summary>
        public int TryGet(string AssetId)
        {
            for (int i = 0; i < InfoList.Count; i++)
            {
                if (InfoList[i].AssetId.Equals(AssetId))
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// 尝试通过AssetId获得资源名
        /// </summary>
        public string TryGetAssetFullName(string AssetId)
        {
            for (int i = 0; i < InfoList.Count; i++)
            {
                if (InfoList[i].AssetId.Equals(AssetId))
                    return InfoList[i].FullName;
            }
            return null;
        }
        /// <summary>
        /// 检测该资源是否存在，如果存在返回索引
        /// </summary>
        /// <param name="AssetFullName"></param>
        /// <returns></returns>
        public int Contains(string AssetFullName)
        {
            if (NameList.Contains(AssetFullName))
            {
                return NameList.IndexOf(AssetFullName);
            }
            return -1;
        }
    }
}
