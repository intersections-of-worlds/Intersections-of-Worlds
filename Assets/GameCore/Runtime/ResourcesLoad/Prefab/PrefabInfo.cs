using Unity.Entities;
using UnityEngine;
namespace GameCore
{
    //TODO:在序列化机制制作时为每个Prefab的每个子实体（包括自身）生成独一的Prefab运行时ID翻盖你序列化
    /// <summary>
    /// Prefab信息，仅在Prefab上使用
    /// </summary>
    public struct PrefabInfo : ISystemStateComponentData
    {
        /// <summary>
        /// Prefab的资源链接
        /// </summary>
        public AssetRef Ref;
    }
}