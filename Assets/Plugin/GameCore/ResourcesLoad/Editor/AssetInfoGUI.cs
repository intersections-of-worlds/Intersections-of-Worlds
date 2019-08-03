using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using GameCore;
[InitializeOnLoad]
public static class AssetInfoGUI
{
    static AssetInfoGUI()
    {
        Editor.finishedDefaultHeaderGUI += drawAssetInfoGUI;
    }
    public static void drawAssetInfoGUI(Editor editor)
    {
        //如果目标对象不是在磁盘中的资源而是场景中的对象，就不进行任何操作
        if (!EditorUtility.IsPersistent(editor.target))
        {
            return;
        }
        var assetPath = AssetDatabase.GetAssetPath(editor.target);
        //获得资源所在Mod
        var ModName = ModsEditor.GetModNameByPath(assetPath);
        //如果没匹配到，就代表该资源不在某个Mod的Assets文件夹下，忽略
        if (ModName == null)
            return;
        var assetName = ModsEditor.GetAssetNameByPath(assetPath);
        AssetInfo ai = ModsEditor.GetAssetInfo(ModName,assetName);
        //赋值
        ai.AssetName = EditorGUILayout.TextField("AssetName", "");
    }
}
