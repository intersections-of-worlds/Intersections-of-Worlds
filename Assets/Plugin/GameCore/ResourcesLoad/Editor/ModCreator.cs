using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore;
using UnityEditor;
public class ModCreatorWindow : EditorWindow
{
    public string ModInternalName;
    [MenuItem("Intersections of Worlds/Mod/CreateMod")]
    public static void OpenModCreatorWindow()
    {
        ModCreatorWindow window = EditorWindow.GetWindow<ModCreatorWindow>();
        window.Show();
    }
    private void OnGUI()
    {
        ModInternalName = EditorGUILayout.TextField("ModInternalName",ModInternalName);
        if (GUILayout.Button("Create"))
        {
            CreateMod();
        }
    }
    private void CreateMod()
    {
        if (ModInternalName == "" || ModInternalName == null)
        {
            Debug.LogError("ModInternalName不能为空");
            return;
        }
        //如果Mods文件夹不存在，则创建
        if (!AssetDatabase.IsValidFolder("Assets/Mods"))
        {
            AssetDatabase.CreateFolder("Assets", "Mods");
        }
        //如果Mod已存在，报错
        if(AssetDatabase.IsValidFolder("Assets/Mods/" + ModInternalName))
        {
            Debug.LogError("该Mod已存在！");
            return;
        }
        var ModPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder("Assets/Mods", ModInternalName));
        //创建ModInfo文件
        var modinfo = CreateInstance<ModInfo>();
        modinfo.name = "ModInfo";
        modinfo.InternalName = ModInternalName;
        AssetDatabase.CreateAsset(modinfo, ModPath + "/" + modinfo.name + ".asset");
        //创建语言文件
        var landic = CreateInstance<LanguageDictionary>();
        landic.name = "LanguageDic";
        AssetDatabase.CreateAsset(landic, ModPath + "/" + landic.name + ".asset");
        //创建AssetIndexer文件
        var indexer = CreateInstance<AssetIndexer>();
        indexer.name = "AssetIndexer";
        AssetDatabase.CreateAsset(indexer, ModPath + "/" + indexer.name + ".asset");
        //创建资源文件夹
        AssetDatabase.CreateFolder(ModPath, "Assets");


        AssetDatabase.Refresh();
        Debug.Log("Mod" + ModInternalName + "创建成功");
        this.Close();
    }
}
