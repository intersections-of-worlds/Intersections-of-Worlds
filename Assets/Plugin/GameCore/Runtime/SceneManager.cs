using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
namespace GameCore
{
    /// <summary>
    /// 场景管理类
    /// </summary>
    [DisableAutoCreation]
    public class SceneManager : ComponentSystem
    {
        /// <summary>
        /// 对象管理器
        /// </summary>
        public WorldObjectManager ObjManager;
        /// <summary>
        /// 场景的id
        /// </summary>
        public int SceneId;
        /// <summary>
        /// 存场景实体
        /// </summary>
        public Entity SceneDataEntity;
        /// <summary>
        /// 场景的Map
        /// </summary>
        public SceneMap Map;
        /// <summary>
        /// 获得场景的基本信息
        /// </summary>
        public SceneData GetSceneData() => EntityManager.GetComponentData<SceneData>(SceneDataEntity);
        /// <summary>
        /// 场景内转换物体的映射表
        /// </summary>
        public Dictionary<Entity, GameObject> ConversationGroup = new Dictionary<Entity, GameObject>();
        public bool IsLoaded { get; private set; }

        protected override void OnUpdate()
        {
            
        }
        public static SceneManager CreateScene(WorldObjectManager objManager,int SceneId,SceneCreator creator)
        {
            var scene = World.Active.CreateSystem<SceneManager>();
            scene.SceneId = SceneId;
            scene.ObjManager = objManager;
            scene.Map = creator.CreateSceneMap(scene);
            scene.CreateSceneDataEntity();
            creator.Create(scene.Map, SaveManager.Active.Info.SaveSeed.Add(SceneId),()=> { scene.IsLoaded = true; });//获得子随机数并创建场景
            return scene;
        }
        
        /// <summary>
        /// 创建场景实体
        /// </summary>
        public void CreateSceneDataEntity()
        {
            if (!(SceneDataEntity.Version == 0 && SceneDataEntity.Index == 0))
                throw new System.Exception("该场景的信息实体已生成！");
            //创建场景实体
           
            SceneDataEntity =  EntityManager.CreateEntity(typeof(LocalToWorld),typeof(Translation),typeof(SceneData), typeof(Identification), typeof(LinkedEntityGroup));
            //设置组件数据
            EntityManager.SetComponentData(SceneDataEntity, new SceneData());
            EntityManager.SetComponentData(SceneDataEntity, new Identification { SceneId = SceneId, ObjectId = 0 });//场景数据实体的objectid为0
            //设置组件位置
            EntityManager.SetComponentData(SceneDataEntity, new Translation { Value = GetScenePostion(SceneId) });
        }
        public void CreateObject(AssetRef ObjectRef,InstantiateCallBack callBack)
        {
            ObjManager.Instantiator.Instantiate(ObjectRef, (e, b) =>
            {
                //设置实体的身份标识
                Identification id = new Identification
                {
                    Asset = ObjectRef,
                    SceneId = SceneId,
                    ObjectId = GetNextObjectId()
                };
                id.SceneId = SceneId;
                id.ObjectId = GetNextObjectId();
                EntityManager.SetComponentData( e, id);
                //将实体添加为场景实体的子实体
                EntityManager.GetBuffer<LinkedEntityGroup>(SceneDataEntity).Add(e);
                EntityManager.SetComponentData(e, new Parent { Value = SceneDataEntity });
                //如果Entity有相对应的GameObject，将其加入转换列表
                if (b)
                {
                    ConversationGroup.Add(e, World.Active.EntityManager.GetComponentObject<Transform>(e).gameObject);
                }
                callBack(e, b);
            });
        }
        public int GetNextObjectId()
        {
            var d = GetSceneData();
            int result = d.GetNextObjectId();
            SaveSceneDataChange(d);
            return result;
        }
        /// <summary>
        /// 保存scenedata的更改
        /// </summary>
        public void SaveSceneDataChange(SceneData data) => EntityManager.SetComponentData(SceneDataEntity, data);
        public float3 GetScenePostion(int SceneId) => new float3(SceneId * SceneCreator.MaxWidth * 2,0,0);
    }
    public struct SceneData : ISystemStateComponentData
    {
        public int NextObjectId;
        public int GetNextObjectId()
        {
            NextObjectId += 1;
            return NextObjectId;
        }
    }
}
