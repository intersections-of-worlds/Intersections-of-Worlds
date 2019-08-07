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
        //获得资源所在Mod
        var ModName = editor.target.GetAssetModNameEditor();
        //如果没匹配到，就代表该资源不在某个Mod的Assets文件夹下，忽略
        if (ModName == null)
            return;
        AssetInfo ai = ModsEditor.GetAssetInfo(ModName,editor.target.name);
        //赋值
        string newAssetName = EditorGUILayout.TextField("AssetName", ai.AssetName);
        if(newAssetName != ai.AssetName)
        {
            ai.AssetName = newAssetName;
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetOrScenePath(editor.target), ai.ModName + "." + ai.AssetName);
        }
        GUILayout.Label("Tags:");
        GUILayout.BeginVertical();
        for (int i = 0; i < ai.Tags.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(ai.Tags[i]);
            if (GUILayout.Button("Remove"))
            {
                ai.Tags.RemoveAt(i);
                EditorUtility.SetDirty(ModsEditor.GetAssetIndexer(ai.ModName));
                AssetDatabase.SaveAssets();
            }
            GUILayout.EndHorizontal();
        }
        if(GUILayout.Button("Add Tag"))
        {
            AddTagWindow window = EditorWindow.GetWindow<AddTagWindow>();
            window.target = ai;
            window.Show();
        }
        GUILayout.EndVertical();
    }
}
public class AddTagWindow : EditorWindow
{
    public AssetInfo target;
    string TagModName = "";
    string TagName = "";
    private void OnGUI()
    {
        TagModName = EditorGUILayout.TextField("Tag's Mod Name", TagModName);
        TagName = EditorGUILayout.TextField("Tag Name", TagName);
        if (GUILayout.Button("Add"))
        {
            target.Tags.Add(TagModName,TagName);
            EditorUtility.SetDirty(ModsEditor.GetAssetIndexer(target.ModName));
            AssetDatabase.SaveAssets();
            Close();
        }
    }
}
