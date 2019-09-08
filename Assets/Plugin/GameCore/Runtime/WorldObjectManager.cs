using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
namespace GameCore
{
    /// <summary>
    /// 世界内所有对象的管理器
    /// </summary>
    [DisableAutoCreation]
    public class WorldObjectManager : ComponentSystem
    {
        

        public static WorldObjectManager Main;
        /// <summary>
        /// 当前存档
        /// </summary>
        SaveManager Save;
        /// <summary>
        /// 当前已加载的场景（key为SceneId）
        /// </summary>
        public Dictionary<int, SceneManager> ExistingScenes = new Dictionary<int, SceneManager>();
        public PrefabInstantiator Instantiator;
        protected override void OnCreate()
        {
            if (Main == null)
                Main = this;
            else
                throw new System.Exception("WorldObjectManager已存在！");
            base.OnCreate();
            Save = SaveManager.Active;
            Instantiator = Save.SystemsManager.Instantiator;
        }
        protected override void OnUpdate()
        {

        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            Main = null;
        }
        /// <summary>
        /// 在世界中创建对象
        /// </summary>
        public void CreateObject(int SceneId,AssetRef ObjectRef,InstantiateCallBack callBack)
        {
            //检测要创建的对象所在的场景是否存在
            if (!ExistingScenes.ContainsKey(SceneId))
                throw new System.ArgumentException("该场景未加载或不存在！", "SceneId");
            ExistingScenes[SceneId].CreateObject(ObjectRef,callBack);
        }
        public void DeleteObject(Entity e)
        {

        }
        public void CreateScene(SceneCreator creator,int SceneId)
        {
            SceneManager sm = SceneManager.CreateScene(this,SceneId, creator);
            ExistingScenes.Add(SceneId,sm);
        }
    }
}

