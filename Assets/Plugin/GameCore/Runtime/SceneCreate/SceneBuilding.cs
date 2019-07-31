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
        /// 该建筑生成的地形
        /// </summary>
        public TerrainDependences dependences;
    }
    /// <summary>
    /// 建筑依赖的地形的信息
    /// </summary>
    [Serializable]
    public class TerrainDependences
    {
        public List<string> All;
        public List<string> Any;
    }

}
