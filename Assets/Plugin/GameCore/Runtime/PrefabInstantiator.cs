using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
namespace GameCore
{
    [DisableAutoCreation]
    public class PrefabInstantiator : ComponentSystem
    {
        /// <summary>
        /// 所有已被生成过的可以不生成GameObject的Prefab对应的Entity
        /// </summary>
        public Dictionary<AssetRef, Entity> Prefabs = new Dictionary<AssetRef, Entity>();
        protected override void OnUpdate()
        {

        }
        /// <summary>
        /// 实例化Prefab
        /// </summary>
        /// <returns>如果该Prefab上有需要保留的MonoBehaviour组件，GameObject则为Prefab实例，否则为null</returns>
        public (Entity,GameObject) Instantiate(AssetRef ObjectRef)
        {
            //如果该Prefab有对应的entity prefab，直接通过该entity prefab生成
            if (Prefabs.ContainsKey(ObjectRef))
            {
                Entity e = EntityManager.Instantiate(Prefabs[ObjectRef]);
                return (e, null);
            }else
            {
                GameObject prefab = ObjectRef.Get<GameObject>();
                GameObject instance = GameObject.Instantiate(prefab);
                Entity e = GameObjectConversionUtility.ConvertGameObjectHierarchy(instance, World);
                //如果该GameObject可以被删除（即没有任何ComponentObject需要添加），删除GameObject并将其对应的Entity Prefab添加到Prefabs字典中
                if (instance.GetComponent<WorldObject>().CanBeDelete())
                {
                    GameObject.Destroy(instance);
                    Entity eprefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, World);
                    EntityManager.AddComponentData<Prefab>(eprefab, new Prefab());
                    Prefabs.Add(ObjectRef,eprefab);
                    return (e, null);
                }
                return (e, instance);
            }
            
        }
    }
}