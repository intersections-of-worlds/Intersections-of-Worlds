using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameCore;
[CustomEditor(typeof(AssetIndexer))]
public class AssetIndexerGUI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var asset = (AssetIndexer)target;
        GUILayout.Label("Dic条数："+asset.NameList.Count);
        int length = asset.NameList.Count;
        for (int i = 0; i < length; i++)
        {
            GUILayout.Label(asset.NameList[i] + ":" + asset.InfoList[i]);
        }
        if (GUILayout.Button("Clear"))
        {
            asset.Clear();
        }
    }
}
