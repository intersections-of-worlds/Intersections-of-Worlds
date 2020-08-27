#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using GameCore;
namespace GameCore {
    /// <summary>
    /// Asset索引器Editor版
    /// </summary>
    public partial class AssetIndexer : ScriptableObject
    {
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
    }
}
#endif