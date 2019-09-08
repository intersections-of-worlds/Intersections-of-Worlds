using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
namespace GameCore
{
    /// <summary>
    /// 所有可被保存的物体都需加上这个组件
    /// </summary>
    [AddComponentMenu("GameCore/WorldObject")]
    public class WorldObject : MonoBehaviour, IConvertGameObjectToEntity
    {
        /// <summary>
        /// 需要在转换后添加到entity的组件
        /// </summary>
        public List<Component> ComponentObjects;
        /// <summary>
        /// 当前GameObject所对应的实体
        /// </summary>
        public Entity entity;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            Debug.Log(entity);
            dstManager.AddComponentData(entity, new Identification ());
            dstManager.AddComponentData(entity, new Parent());
            dstManager.AddComponentData(entity, new LocalToParent());
            if (CanBeDelete()) {//如果GameObject可在转换后被删除，CopyTransformToGameObject以及添加ComponentObject的步骤就没有必要了
                return;
            }
            dstManager.AddComponentData(entity, new CopyTransformToGameObject());
            for (int i = 0; i < ComponentObjects.Count; i++)
            {
                dstManager.AddComponentObject(entity, ComponentObjects[i]);
            }

            this.entity = entity;
        }
        void Awake()
        {
            ComponentObjects = new List<Component>();
            //从当前GameObject的组件中找出需要作为ComponentObject添加到转换后的entity的组件
            Component[] components = GetComponents<Component>();
            for(int i = 0; i < components.Length; i++)
            {
                //如果该组件会被转换成ComponentData，无视
                if(components[i] is IConvertGameObjectToEntity)
                {
                    //Transform除外
                    if(components[i] is Transform)
                    {
                        ComponentObjects.Add(components[i]);
                    }
                    continue;
                }
                //如果该组件有自定义的转换系统，无视
                if (components[i] is MeshRenderer || components[i] is MeshFilter)
                    continue;
                ComponentObjects.Add(components[i]);
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
        public bool CanBeDelete()
        {
            return ComponentObjects.Count <= 1; //如果组件列表只有一个组件（即transform），就代表该GameObject可在转换后删除，
        }
    }
}
