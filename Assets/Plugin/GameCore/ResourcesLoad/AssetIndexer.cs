using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
            EditorUtility.SetDirty(this);
        }
        public void Remove(string assetFullName)
        {
            Init();
            InfoList.RemoveAt(NameList.IndexOf(assetFullName));
            NameList.Remove(assetFullName);
            EditorUtility.SetDirty(this);
        }
        /// <summary>
        /// 重命名资源
        /// </summary>
        /// <param name="Old">旧资源名</param>
        /// <param name="New">新资源名</param>
        public void Rename(string Old, string New)
        {
            NameList[NameList.IndexOf(Old)] = New;
            InfoList[NameList.IndexOf(New)].AssetName = AssetUtility.GetAssetName(New);
        }
        public void Clear()
        {
            NameList.Clear();
            InfoList.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
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
        /// <summary>
        /// 尝试通过AssetId获得资源，没成功返回-1
        /// </summary>
        public int TryGet(int AssetId)
        {
            for (int i = 0; i < InfoList.Count; i++)
            {
                if (InfoList[i].AssetId == AssetId)
                    return i;
            }
            return -1;
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
