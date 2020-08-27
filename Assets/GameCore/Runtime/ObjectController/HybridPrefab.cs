using Unity.Entities;
using UnityEngine;

namespace GameCore
{
    /// <summary>
    /// 混合Prefab
    /// </summary>
    [AddComponentMenu("GameCore/HybridPrefab")]
    public class HybridPrefab : MonoBehaviour, IConvertGameObjectToEntity
    {
        /// <summary>
        /// 要保留的组件
        /// </summary>
        [SerializeField]
        private MonoBehaviour[] ComponentsToKeep = null;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem system)
        {
            //挨个把组件设为保留
            if (ComponentsToKeep != null)
            {
                for (int i = 0; i < ComponentsToKeep.Length; i++)
                {
                    system.AddHybridComponent(ComponentsToKeep[i]);
                }
            }
            
        }
    }
}