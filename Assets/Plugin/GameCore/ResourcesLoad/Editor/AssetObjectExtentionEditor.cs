using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameCore;
public static class AssetObjectExtentionEditor
{
    /// <summary>
    /// 获得资源的信息（请不要在一个不是Mod资源的对象上调用此函数）
    /// </summary>
    public static AssetInfo GetAssetInfoEditor<T>(this T asset) where T : UnityEngine.Object
    {
        return ModsEditor.GetAssetInfo(asset.GetAssetModNameEditor(), asset.GetAssetNameEditor());
    }
    /// <summary>
    /// 获得资源所属Mod的名称（请不要在一个不是Mod资源的对象上调用此函数）
    /// </summary>
    public static string GetAssetModNameEditor<T>(this T asset) where T : UnityEngine.Object
    {
        return AssetUtility.GetModName(asset.name);
    }
    /// <summary>
    /// 获得资源的名称（请不要在一个不是Mod资源的对象上调用此函数）
    /// </summary>
    public static string GetAssetNameEditor<T>(this T asset) where T : UnityEngine.Object
    {
        return AssetUtility.GetAssetName(asset.name);
    }
    /// <summary>
    /// 获得资源所属Mod的id
    /// </summary>
    public static int GetModIdEditor<T>(this T asset) where T : UnityEngine.Object
    {
        return ModsEditor.GetModId(asset.GetAssetModNameEditor());
    }
}
