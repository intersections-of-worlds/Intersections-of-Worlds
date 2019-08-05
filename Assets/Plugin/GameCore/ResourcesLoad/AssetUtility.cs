using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameCore
{
    public static class AssetUtility
    {
        /// <summary>
        /// 从资源的全名中分解出资源的名称
        /// </summary>
        public static string GetAssetName(string AssetFullName)
        {
            var s = AssetFullName.Split('.');
            string assetName = "";
            for(int i = 1; i < s.Length; i++)
            {
                assetName += s[i];
            }
            return assetName;
        }
        /// <summary>
        /// 从资源的全名中分解出资源的Mod名
        /// </summary>
        /// <param name="AssetFullName"></param>
        /// <returns></returns>
        public static string GetModName(string AssetFullName)
        {
            var s = AssetFullName.Split('.');
            if (s.Length == 1) return null;//如果名称中只有一个.直接返回null
            return s[0];
        }
    }
}
