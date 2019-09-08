using UnityEngine;
using System;
namespace GameCore
{
    [Serializable]
    [CreateAssetMenu(menuName = "Intersections of Worlds/Scene/SceneBiome")]
    public class SceneBiome : ScriptableObject
    {
        /// <summary>
        /// 群系的名称
        /// </summary>
        public string BiomeName { get => this.GetAssetName(); }
        /// <summary>
        /// 群系创建者的名称（C#为类名，Lua为文件名）
        /// </summary>
        public string BiomeCreatorName;
        /// <summary>
        /// 该群系生成所依赖的地形
        /// </summary>
        public TerrainDependences dependences;
    }
}
