using UnityEngine;
using System;
using System.Collections.Generic;
namespace GameCore
{
    [Serializable]
    [CreateAssetMenu(menuName = "Intersections of Worlds/Scene/SceneBuilding")]
    public class SceneBuilding : ScriptableObject
    {
        public string BuildingName { get => this.GetAssetName(); }
        public string BuildingCreatorName;
        /// <summary>
        /// 该建筑生成所依赖的地形
        /// </summary>
        public TerrainDependences dependences;
        /// <summary>
        /// 该建筑生成的概率（每100*100的区域中）
        /// </summary>
        [Range(0f,1f)]
        public float probability;
        public float probabilityPerTile { get => probability * 10000; }
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
        public string[] All;
        /// <summary>
        /// 建筑所在区域可以包含的地形
        /// </summary>
        public string[] Any;
        /// <summary>
        /// 检测这些地形是否满足依赖
        /// </summary>
        public bool IsMatch(List<string> Terrains)
        {
            for(int i = 0; i < All.Length; i++)
            {
                //Terrains必须包含All的所有内容
                if (!Terrains.Contains(All[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }

}
