using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameCore;
using System.Text.RegularExpressions;
/// <summary>
/// 编辑器下的Mod操作类
/// </summary>
public static class ModsEditor
{
    /// <summary>
    /// 获得指定Mod的AssetIndexer
    /// </summary>
    public static AssetIndexer GetAssetIndexer(string ModName)
    {
        return AssetDatabase.LoadAssetAtPath<AssetIndexer>("Assets/Mods/" + ModName + "/AssetIndexer.asset");
    }
    /// <summary>
    /// 获得指定资源的信息
    /// </summary>
    public static AssetInfo GetAssetInfo(string ModName,string AssetName)
    {
        return GetAssetIndexer(ModName).InfoDic[AssetName];
    }
    /// <summary>
    /// 通过资源路径获得Mod名称
    /// </summary>
    /// <param name="assetPath">资源路径</param>
    /// <returns>Mod名称，如果该资源不在任何一个Mod下则返回null</returns>
    public static string GetModNameByPath(string assetPath)
    {
        var Match = Regex.Match(assetPath, @"^Assets/Mods/([a-zA-Z0-9]+)/Assets/");
        if (Match.Success)
        {
            return Match.Groups[1].Value;
        }
        return null;

    }
    /// <summary>
    /// 获得一个Mod中的资源的全名
    /// </summary>
    /// <param name="assetPath">资源路径</param>
    /// <returns>资源名称，如果该资源不在Mod中则为null</returns>
    public static string GetAssetFullNameByPath(string assetPath)
    {
        if (IsInMod(assetPath))
        {
            //分解路径以获得资源名
            var split = assetPath.Split('/');
            //获得资源的文件名
            var FileName = split[split.Length - 1];
            //去后缀处理
            split = FileName.Split('.');
            string assetName = "";
            for (int i = 0; i < split.Length - 1; i++)
            {
                assetName += split[i];
            }
            return assetName;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 获得一个Mod中的资源的名称
    /// </summary>
    /// <param name="assetPath">资源路径</param>
    /// <returns>资源名称，如果该资源不在Mod中则为null</returns>
    public static string GetAssetNameByPath(string assetPath)
    {
        string FullName = GetAssetFullNameByPath(assetPath);
        if (FullName == null)
            return null;
        return AssetUtility.GetAssetName(FullName);
    }
    public static int GetModId(string ModName)
    {
        return GetModInfo(ModName).ModId;
    }
    /// <summary>
    /// 由Mod名获得ModInfo
    /// </summary>
    public static ModInfo GetModInfo(string ModName)
    {
        var ModInfoPath = "Assets/Mods/" + ModName + "/ModInfo.asset";
        return AssetDatabase.LoadAssetAtPath<ModInfo>(ModInfoPath);
    }
    /// <summary>
    /// 检测一个资源是否在一个Mod下
    /// </summary>
    /// <param name="assetPath">资源路径</param>
    public static bool IsInMod(string assetPath)
    {
        return Regex.Match(assetPath, @"^Assets/Mods/[a-zA-Z0-9]+/Assets/").Success;
    }
    
}
