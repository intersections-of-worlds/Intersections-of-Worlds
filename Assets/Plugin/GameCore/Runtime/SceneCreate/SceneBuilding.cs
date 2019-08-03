using UnityEngine;
using System;
using System.Collections.Generic;
namespace GameCore
{
    [Serializable]
    [CreateAssetMenu(menuName = "Intersections of Worlds/Scene/SceneBuilding")]
    public class SceneBuilding : ScriptableObject
    {
        public string BuildingName;
        /// <summary>
        /// 该建筑生成所依赖的地形
        /// </summary>
        public TerrainDependences dependences;
    }
    /// <summary>
    /// 建筑依赖的地形的信息
    /// </summary>
    [Serializable]
    public class TerrainDependences
    {
        /// <summary>
        /// 建筑所在区域必须包含的地形
        /// </summary>
        public List<string> All;
        /// <summary>
        /// 建筑所在区域可以包含的地形
        /// </summary>
        public List<string> Any;
    }

}
