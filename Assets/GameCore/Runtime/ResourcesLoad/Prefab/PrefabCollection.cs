using System;
using Unity.Entities;
using System.Collections.Generic;

namespace GameCore{
    //TODO:加个具体Prefab instanceID（即Prefab的子物体的ID）及对应关系系统
    /// <summary>
    /// 储存世界中所有Prefab的集合
    /// </summary>
    public class PrefabCollection : IComponentData {
        /// <summary>
        /// 通过AssetRef获得对应Entity
        /// </summary>
        protected Dictionary<AssetRef,Entity> RefToEntity = new Dictionary<AssetRef,Entity>();
        /// <summary>
        /// 通过Entity获得对应的AssetRef
        /// </summary>
        protected Dictionary<Entity,AssetRef> EntityToRef = new Dictionary<Entity,AssetRef>();
        /// <summary>
        /// 通过AssetRef获得对应Entity
        /// </summary>
        public Entity this[AssetRef Ref]{
            get => RefToEntity[Ref];
        }
        /// <summary>
        /// 通过Entity获得对应的AssetRef
        /// </summary>
        public AssetRef this[Entity prefab]{
            get => EntityToRef[prefab];
        }
        /// <summary>
        /// 添加一个Prefab
        /// </summary>
        /// <param name="Ref">Prefab资源的引用</param>
        /// <param name="prefab">Prefab</param>
        public void Add(AssetRef Ref,Entity prefab){
            RefToEntity.Add(Ref,prefab);
            EntityToRef.Add(prefab,Ref);
            
        }
        /// <summary>
        /// 是否包含某个资源对应的Prefab
        /// </summary>
        public bool Contains(AssetRef Ref){
            return RefToEntity.ContainsKey(Ref);
        }
    }
}