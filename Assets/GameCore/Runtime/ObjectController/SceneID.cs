using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;

namespace GameCore{
    /// <summary>
    /// 游戏对象所属场景的Id，当游戏对象位置变更会发生改变
    /// </summary>
    public struct SceneId : ISharedComponentData//为筛选快捷方便将其设共享组件
    {
        //Scene不用担心超界
        public int Id;
        /// <summary>
        /// 指向管理该Scene的实体
        /// </summary>
        public Entity SceneEntity;
    }
    /// <summary>
    /// 用于给目标添加SceneId组件
    /// </summary>
    [AddComponentMenu("GameCore/SceneIdConverter")]
    public class SceneIdConverter : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstmanager, GameObjectConversionSystem system)
        {
            dstmanager.AddSharedComponentData<SceneId>(entity, new SceneId());
        }
    }
}