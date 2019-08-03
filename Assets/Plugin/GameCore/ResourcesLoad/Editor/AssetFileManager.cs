using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameCore;

public class AssetFileManager : AssetPostprocessor
{

    static void OnPostprocessAllAssets(string[] importedAssets,string[] deletedAssets,string[] movedAssets,string[] movedFromAssetsPaths)
    {
        //先处理导入的资源
        for(int i = 0; i < importedAssets.Length; i++)
        {
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(importedAssets[i]);
            var ModName = ModsEditor.GetModNameByPath(importedAssets[i]);
            //如果该资源不属于任何一个Mod，无视
            if(ModName == null)
            {
                continue;
            }
            ImportAsset(ModName,asset);
        } 
        //再处理删除的资源
        for (int i = 0; i < deletedAssets.Length; i++)
        {
            var ModName = ModsEditor.GetModNameByPath(deletedAssets[i]);
            //如果该资源不属于任何一个Mod，无视
            if (ModName == null)
            {
                continue;
            }
            var AssetName = ModsEditor.GetAssetNameByPath(deletedAssets[i]);
            DeleteAsset(ModName, AssetName);
        }
        //最后处理移动的资源
        for(int i = 0; i < movedAssets.Length;i++)
        {
            var FromMod = ModsEditor.GetModNameByPath(movedFromAssetsPaths[i]);
            var ToMod = ModsEditor.GetModNameByPath(movedAssets[i]);
            var assetName = ModsEditor.GetAssetNameByPath(movedAssets[i]);//用哪个数组都无所谓，反正同步的
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(movedAssets[i]);
            //如果没有移动到另一个Mod，无视（都为null，即并非Mod之间的资源移动，也无视）
            if (FromMod == ToMod)
            {
                continue;
            }
            //ToMod为null即为从Mod中导出资源
            if(ToMod == null)
            {
                DeleteModName(asset);
                DeleteAsset(FromMod, assetName);
                continue;
            }
            //FromMod为null即为导入资源到Mod中
            if(FromMod == null)
            {
                ImportAsset(ToMod,asset);
                continue;
            }
            //最后处理把资源从Mod移动到Mod的情况
            MoveAsset(FromMod, ToMod, assetName,asset);
        }
        Debug.Log("资源处理成功");
    }
    /// <summary>
    /// 导入资源到Mod时触发的事件
    /// </summary>
    static void ImportAsset(string ModName,UnityEngine.Object asset)
    {
        AddModName(ModName, asset);
        AssetIndexer ai = ModsEditor.GetAssetIndexer(ModName);
        ai.Add(asset.name,CreatAssetInfo(asset));
    }
    /// <summary>
    /// 从Mod中移除资源时触发的事件
    /// </summary>
    static void DeleteAsset(string ModName,string AssetName)
    {
        AssetIndexer ai = ModsEditor.GetAssetIndexer(ModName);
        ai.Remove(ModName + "." + AssetName);
    }
    /// <summary>
    /// 移动Mod资源到另一个Mod时调用
    /// </summary>
    static void MoveAsset(string FromMod,string ToMod,string AssetName,UnityEngine.Object asset)
    {
        DeleteModName(asset);
        AssetIndexer FromModai = ModsEditor.GetAssetIndexer(FromMod);
        var assetInfo = FromModai.InfoDic[FromMod + "." + AssetName];
        FromModai.Remove(FromMod + "." + AssetName);

        AddModName(ToMod, asset);
        assetInfo.ModName = ToMod;
        assetInfo.ModId = ModsEditor.GetModId(ToMod);
        AssetIndexer ToModai = ModsEditor.GetAssetIndexer(ToMod);
        ToModai.Add(asset.name, assetInfo);
    }
    /// <summary>
    /// 把名称中的ModName删除，请务必在删除AssetInfo索引之前调用
    /// </summary>
    static void DeleteModName(UnityEngine.Object asset)
    {
        asset.name = asset.GetAssetNameEditor();
        EditorUtility.SetDirty(asset);
    }
    /// <summary>
    /// 在名称中加上ModName，请务必在添加AssetInfo索引之前调用
    /// </summary>
    static void AddModName(string ModName,UnityEngine.Object asset)
    {
        asset.name = ModName + "." + asset.name;
        EditorUtility.SetDirty(asset);
    }
    /// <summary>
    /// 创建AssetInfo
    /// </summary>
    /// <returns></returns>
    public static AssetInfo CreatAssetInfo(UnityEngine.Object asset)
    {
        return new AssetInfo(asset.GetAssetModNameEditor(), asset.GetAssetNameEditor(),
            ModsEditor.GetModId(asset.GetAssetModNameEditor()), asset.GetAssetIdEditor(), asset.GetTypeName());
    }
}
