using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System;
namespace GameCore {
    /// <summary>
    /// Entity标识，只有拥有该标识的entity才能进行储存等操作
    /// </summary>
    public struct Identification : IComponentData, IEquatable<Identification>
    {
        /// <summary>
        /// 所在场景id，当为0时即未初始化
        /// </summary>
        public int SceneId;
        /// <summary>
        /// 当前物体id，为0时未初始化
        /// </summary>
        public int ObjectId;
        /// <summary>
        /// 该对象所属资源类型
        /// </summary>
        public AssetRef Asset;
        public Identification(int sceneId,int objectId,AssetRef asset)
        {
            SceneId = sceneId;
            ObjectId = objectId;
            Asset = asset;
        }

        public bool Equals(Identification other)
        {
            return this.SceneId == other.SceneId && ObjectId == other.ObjectId;
        }
    }
}
