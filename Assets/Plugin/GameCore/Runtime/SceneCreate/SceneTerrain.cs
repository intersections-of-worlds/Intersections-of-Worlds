using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameCore
{
    [Serializable]
    [CreateAssetMenu(menuName = "Intersections of Worlds/Scene/SceneTerrain")]
    public class SceneTerrain : ScriptableObject
    {
        /// <summary>
        /// 地形的名称，内部名
        /// </summary>
        public string TerrainName;
        /// <summary>
        /// 地形创建者的名称（C#为类名，Lua为文件名）
        /// </summary>
        public string TerrainCreatorName;
        public void Create()
        {

        }
    }

}
