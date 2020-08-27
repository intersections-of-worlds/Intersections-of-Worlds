using Unity.Entities;
using UnityEngine;
using System;

namespace GameCore
{
    /// <summary>
    /// 用于管理Prefab的系统，重写了默认系统组的行为，将在有Prefab添加时Update
    /// </summary>
    [UpdateInWorld(WorldTypes.ClientWorld | WorldTypes.ServerWorld)]
    public class PrefabManager : ComponentSystemGroup
    {
        /// <summary>
        /// 所在World的类型
        /// </summary>
        protected WorldTypes worldType;
        /// <summary>
        /// 已加载Prefab的集合
        /// </summary>
        protected PrefabCollection collection;
        /// <summary>
        /// 转换GO时使用的，目前用途不明，在系统销毁时释放
        /// </summary>
        protected BlobAssetStore store;
        protected override void OnCreate(){
            base.OnCreate();
            //获取世界类型
            worldType = GetSingleton<WorldTypeInfo>().type;
            //创建储存已加载Prefab的实体
            var e = EntityManager.CreateEntity(ComponentType.ReadWrite<PrefabCollection>());
            collection = new PrefabCollection();
            EntityManager.AddComponentObject(e,collection);
            //初始化store
            store = new BlobAssetStore();
        }
        protected override void OnUpdate(){
            //覆盖默认Update行为，当有Prefab创建调用base.OnUpdate()
        }
        /// <summary>
        /// 获取Prefab
        /// </summary>
        public Entity GetPrefab(AssetRef Ref){
            //如果Prefab已经加载出来了，直接返回，如果没有，就加载
            if(collection.Contains(Ref)){
                return collection[Ref];
            }else{
                return LoadPrefab(ref Ref);
            }
        }
        /// <summary>
        /// 将一个Prefab加载并转换成Entity
        /// </summary>
        protected Entity LoadPrefab(ref AssetRef Ref){//用ref是为了减少开销，虽然估计也没减少多少，回头可能会删？
            var prefabGO = Ref.Get<GameObject>();
            if(prefabGO == null){
                throw new ArgumentException("Ref的类型必须是GameObject","Ref");
            }
            //转换Prefab
            var prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabGO,
                GameObjectConversionSettings.FromWorld(this.World,store));
            //为Prefab实体添加Prefab信息组件
            EntityManager.AddComponentData(prefabEntity,new PrefabInfo(){Ref = Ref});
            EntityManager.AddComponentData(prefabEntity,new InitializingTag());
            //Update系统组，对新的Prefab实体进行处理
            this.Update();
            EntityManager.RemoveComponent<InitializingTag>(prefabEntity);
            return prefabEntity;
        }
        protected override void OnDestroy(){
            //释放
            store.Dispose();
        }
    }
}