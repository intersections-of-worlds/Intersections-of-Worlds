using UnityEngine;
using System;
namespace GameCore
{
    public static class AssetObjectExtention
    {
        /// <summary>
        /// 获得资源的信息（请不要在一个不是Mod资源的对象上调用此函数）
        /// </summary>
        public static AssetInfo GetAssetInfo<T>(this T asset) where T : UnityEngine.Object
        {
            return SaveManager.Active.GetAssetInfo(asset.name);
        }
        /// <summary>
        /// 获得资源的引用（请不要在一个不是Mod资源的对象上调用此函数）
        /// </summary>
        public static AssetRef GetAssetRef<T>(this T asset) where T : UnityEngine.Object
        {
            return asset.GetAssetInfo().GetRef();
        }
        /// <summary>
        /// 获得资源所属Mod的名称（请不要在一个不是Mod资源的对象上调用此函数）
        /// </summary>
        public static string GetAssetModName<T>(this T asset) where T : UnityEngine.Object
        {
            return AssetUtility.GetModName(asset.name);
        }
        /// <summary>
        /// 获得资源的名称（请不要在一个不是Mod资源的对象上调用此函数）
        /// </summary>
        public static string GetAssetName<T>(this T asset) where T : UnityEngine.Object
        {
            return AssetUtility.GetAssetName(asset.name);
        }
        /// <summary>
        /// 获得资源的类型名
        /// </summary>
        public static string GetTypeName<T>(this T asset) where T : UnityEngine.Object
        {
            return asset.GetType().FullName;
        }
    }
}
