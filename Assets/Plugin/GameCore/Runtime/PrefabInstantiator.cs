using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
namespace GameCore
{

    /// <summary>
    /// 实例化的回调
    /// </summary>
    /// <param name="e">实例化得到的实体</param>
    /// <param name="HaveGameObject">实例化后是否会保留对应的GameObject</param>
    /// <param name="ecb">用来对实体进行操作的ECB</param>
    /// <param name="index">操作时需输入的jobindex</param>
    public delegate void InstantiateCallBack(Entity e, bool HaveGameObject);
    /// <summary>
    /// 集中性实例化任务
    /// </summary>
    public class InstantiateTask : Queue<(AssetRef,InstantiateCallBack)>
    {
        /// <summary>
        /// 任务结束后的回调
        /// </summary>
        protected Action CallBack;
        public InstantiateTask(Action callBack)
        {

        }
        public void Finish()
        {
            CallBack?.Invoke();
        }
    }
    /// <summary>
    /// 用于实例化预制件的系统
    /// </summary>
    [DisableAutoCreation]
    public class PrefabInstantiator : ComponentSystem
    {
        /// <summary>
        /// 所有可以不生成GameObject的Prefab对应的Entity
        /// </summary>
        public Dictionary<AssetRef, Entity> EntityPrefabs = new Dictionary<AssetRef, Entity>();
        /// <summary>
        /// 所有必须生成GameObject的Prefab
        /// </summary>
        public Dictionary<AssetRef, GameObject> GOPrefabs = new Dictionary<AssetRef, GameObject>();
        /// <summary>
        /// 获取ECB的系统
        /// </summary>
        protected EndSimulationEntityCommandBufferSystem ECBSystem;
        /// <summary>
        /// 零散的实例化任务，优先完成
        /// </summary>
        protected InstantiateTask ObjectsToInstantiate = new InstantiateTask(null);
        /// <summary>
        /// 大型实例化任务的列表，按顺序完成
        /// </summary>
        protected Queue<InstantiateTask> InstantiateTasks = new Queue<InstantiateTask>();
        /// <summary>
        /// 当前正在执行的大型实例化任务
        /// </summary>
        protected InstantiateTask InstantiatingTask = new InstantiateTask(null);
        /// <summary>
        /// 每个entity实例化的对应时间
        /// </summary>
        protected const float TimePerEntity = 0.0001f;
        /// <summary>
        /// 每个GO实例化的对应时间
        /// </summary>
        protected const float TimePerGO = 0.01f;
        /// <summary>
        /// 上一帧多余的时间（实际上是负数，即溢出的时间）
        /// </summary>
        protected float RestTime;

        protected override void OnCreate()
        {
            base.OnCreate();
            ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }
        protected bool ShouldRun()
        {
            return ObjectsToInstantiate.Count > 0 || InstantiatingTask.Count > 0 || InstantiateTasks.Count > 0;
        }
        protected override void OnUpdate()
        {
            if(OriginalTask != null)
            {
                throw new Exception("调用了BeginCreateInstantiateTask却没有调用EndCreateInstantiateTask！");
            }
            if (ShouldRun())
            {
                RestTime += Time.deltaTime;
                Debug.Log(RestTime);
                //先处理零散实例化任务
                for (int i = ObjectsToInstantiate.Count - 1; i >= 0; i--)
                {
                    //如果剩余时间用完了，返回
                    if (RestTime < 0)
                    {
                        return;
                    }
                    var task = ObjectsToInstantiate.Dequeue();
                    //如果是可直接实例化为实体的预制件，将其实例化为实体
                    if (EntityPrefabs.ContainsKey(task.Item1))
                    {
                        InstantiateEntity(EntityPrefabs[task.Item1], task.Item2);
                        continue;
                    }
                    //如果是实例化后要保留GameObject的预制件，将其实例化为GameObject
                    if (GOPrefabs.ContainsKey(task.Item1))
                    {
                        InstantiateGameObject(GOPrefabs[task.Item1], task.Item2);
                        continue;
                    }
                    //如果不知道该预制件要不要保留GameObject，检测
                    var Prefab = task.Item1.Get<GameObject>();
                    if (Prefab.GetComponent<WorldObject>().CanBeDelete())
                    {
                        Debug.Log("添加预制件转换任务");
                        //如果不用保留GameObject就先创建其EntityPrefab再实例化
                        CreateEntityPrefab(task.Item1, Prefab);
                        InstantiateEntity(EntityPrefabs[task.Item1], task.Item2);
                    }
                    else
                    {
                        //如果需要保留GameObject，将其记录并实例化为GameObject
                        GOPrefabs.Add(task.Item1, Prefab);
                        InstantiateGameObject(Prefab, task.Item2);
                    }
                }
                //再处理集中实例化任务
                do
                {
                    if (InstantiatingTask.Count == 0)
                    {
                        InstantiatingTask.Finish();
                        InstantiatingTask = InstantiateTasks.Dequeue();
                    }
                    for (int i = InstantiatingTask.Count - 1; i >= 0; i--)
                    {
                        //如果剩余时间用完了，返回
                        if (RestTime < 0)
                        {
                            return;
                        }
                        var task = InstantiatingTask.Dequeue();
                        //如果是可直接实例化为实体的预制件，将其实例化为实体
                        if (EntityPrefabs.ContainsKey(task.Item1))
                        {
                            InstantiateEntity(EntityPrefabs[task.Item1], task.Item2);
                            continue;
                        }
                        //如果是实例化后要保留GameObject的预制件，将其实例化为GameObject
                        if (GOPrefabs.ContainsKey(task.Item1))
                        {
                            InstantiateGameObject(GOPrefabs[task.Item1], task.Item2);
                            continue;
                        }
                        //如果不知道该预制件要不要保留GameObject，检测
                        var Prefab = task.Item1.Get<GameObject>();
                        if (Prefab.GetComponent<WorldObject>().CanBeDelete())
                        {
                            Debug.Log("添加预制件转换任务");
                            //如果不用保留GameObject就先创建其EntityPrefab再实例化
                            CreateEntityPrefab(task.Item1, Prefab);
                            InstantiateEntity(EntityPrefabs[task.Item1], task.Item2);
                        }
                        else
                        {
                            //如果需要保留GameObject，将其记录并实例化为GameObject
                            GOPrefabs.Add(task.Item1, Prefab);
                            InstantiateGameObject(Prefab, task.Item2);
                        }
                    }
                } while (InstantiateTasks.Count != 0);
            }
        }
        protected void InstantiateEntity(Entity prefab,InstantiateCallBack callBack)
        {
            var e = EntityManager.Instantiate(prefab);
            callBack(e, false);
            RestTime -= TimePerEntity;
        }
        protected void CreateEntityPrefab(AssetRef PrefabRef,GameObject Prefab)
        {
            Entity eprefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab, World);
            EntityManager.AddComponentData(eprefab, new Prefab());
            EntityPrefabs.Add(PrefabRef, eprefab);
            RestTime -= TimePerGO;
        }

        
        protected void InstantiateGameObject(GameObject Prefab, InstantiateCallBack callBack)
        {
            GameObject instance = GameObject.Instantiate(Prefab);
            Entity e = GameObjectConversionUtility.ConvertGameObjectHierarchy(instance, World);
            callBack(e, true);
            RestTime -= TimePerGO;
        }
        /// <summary>
        /// 实例化Prefab
        /// </summary>
        public void Instantiate(AssetRef ObjectRef,InstantiateCallBack CallBack)
        {
            ObjectsToInstantiate.Enqueue((ObjectRef, CallBack));
        }
        public void AddInstantiateTask(InstantiateTask task)
        {
            InstantiateTasks.Enqueue(task);
        }
        private InstantiateTask OriginalTask;
        public void BeginCreateInstantiateTask(Action taskCallBack)
        {
            OriginalTask = ObjectsToInstantiate;
            ObjectsToInstantiate = new InstantiateTask(taskCallBack);
        }

        public void EndCreateInstantiateTask()
        {
            InstantiateTasks.Enqueue(ObjectsToInstantiate);
            ObjectsToInstantiate = OriginalTask;
            OriginalTask = null;
        }
        /*
        /// <summary>
        /// 用于实例化纯Entity的Job
        /// </summary>
        struct InstantiateJob : IJobParallelFor
        {
            public EntityCommandBuffer.Concurrent command;
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<Entity> ObjectsToInstantiate;
            public NativeArray<Entity> InstantiatedEntity;
            public void Execute(int index)
            {
                var e = command.Instantiate(index,ObjectsToInstantiate[index]);
                InstantiatedEntity[index] = e;
            }
        }*/
    }
}