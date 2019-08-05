using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameCore
{
    [Serializable]
    public class AssetIndexer : ScriptableObject
    {
        /*/// <summary>
        /// 资源名和资源信息的对应表
        /// </summary>
        public Dictionary<string, AssetInfo> InfoDic;
        public void Add(string assetFullName,AssetInfo asset)
        {
            if(InfoDic == null)
            {
                InfoDic = new Dictionary<string, AssetInfo>();
            }
            InfoDic.Add(assetFullName,asset);
        }
        public void Remove(string assetFullName)
        {
            InfoDic.Remove(assetFullName);
        }*/
        public List<string> NameList;
        public List<AssetInfo> InfoList;

        public void Init()
        {
            if (NameList == null)
            {
                NameList = new List<string>();
                InfoList = new List<AssetInfo>();
            }
        }
#if UNITY_EDITOR
        public void Add(string assetFullName, AssetInfo asset)
        {
            Init();
            NameList.Add(assetFullName);
            InfoList.Add(asset);
            UnityEditor.EditorUtility.SetDirty(this);
        }
        public void Remove(string assetFullName)
        {
            Init();
            InfoList.RemoveAt(NameList.IndexOf(assetFullName));
            NameList.Remove(assetFullName);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
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
    }
}
