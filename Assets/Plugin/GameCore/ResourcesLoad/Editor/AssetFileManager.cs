﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameCore;

public class AssetFileManager : AssetPostprocessor
{
    static int repeat;
    static void OnPostprocessAllAssets(string[] importedAssets,string[] deletedAssets,string[] movedAssets,string[] movedFromAssetsPaths)
    {
        repeat += 1;
        if (repeat > 100)
            return;
        //先处理导入的资源
        for (int i = 0; i < importedAssets.Length; i++)
        {
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(importedAssets[i]);
            var ModName = ModsEditor.GetModNameByPath(importedAssets[i]);
            //如果该资源不属于任何一个Mod，无视
            if (ModName == null)
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
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(movedAssets[i]);
            var FromMod = ModsEditor.GetModNameByPath(movedFromAssetsPaths[i]);
            var ToMod = ModsEditor.GetModNameByPath(movedAssets[i]);
            var assetName = asset.GetAssetNameEditor();
            
            //如果没有移动到另一个Mod，无视（都为null，即并非Mod之间的资源移动，也无视）
            if (FromMod == ToMod)
            {
                continue;
            }
            if(FromMod == null)
            {
                ImportAsset(ToMod, asset);
                continue;
            }
            if (ToMod == null)
            {
                AssetDatabase.RenameAsset(movedAssets[i], assetName);
                DeleteAsset(FromMod, assetName);
                continue;
            }
            var FromModAI = ModsEditor.GetAssetIndexer(FromMod);
            string newFullName = ToMod + "." + assetName;
            var Info = FromModAI[asset.name];
            FromModAI.Remove(asset.name);
            Info.ModName = ToMod;
            Info.ModId = ModsEditor.GetModId(ToMod);
            ModsEditor.GetAssetIndexer(ToMod).Add(newFullName, Info);
            AssetDatabase.RenameAsset(movedAssets[i], newFullName);

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 导入资源到Mod时触发的事件
    /// </summary>
    static void ImportAsset(string ModName,UnityEngine.Object asset)
    {
        //如果该资源名称未处理过，先处理名称，然后让它在下一轮Import事件处理（重命名会重新Import）
        if (asset.GetAssetModNameEditor() != ModName)
        {
            AddModName(ModName, asset);
            return;
        }
        AssetIndexer ai = ModsEditor.GetAssetIndexer(ModName);
        //检测该资源是否已经存在
        int assetindex = ai.TryGet(asset.GetInstanceID());
        if (assetindex == -1)
        {
            ai.Add(asset.name, CreatAssetInfo(asset));
        }
        else
        {
            if (ai.NameList[assetindex] == asset.name)
                return;//没被重命名就无视
            ai.Rename(ai.NameList[assetindex], asset.name);
        }
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
    /// 将旧的ModName换成新的ModName
    /// </summary>
    static void ChangeModName(string NewModName,UnityEngine.Object asset)
    {
        AssetDatabase.RenameAsset(AssetDatabase.GetAssetOrScenePath(asset), asset.GetAssetNameEditor() + NewModName);
    }
    /// <summary>
    /// 在名称中加上ModName，请务必在添加AssetInfo索引之前调用
    /// </summary>
    static void AddModName(string ModName,UnityEngine.Object asset)
    {
        AssetDatabase.RenameAsset(AssetDatabase.GetAssetOrScenePath(asset), ModName + "." + asset.name);
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
