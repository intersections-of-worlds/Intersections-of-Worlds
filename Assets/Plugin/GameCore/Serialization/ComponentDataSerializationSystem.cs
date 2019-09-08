using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

namespace GameCore
{
    [DisableAutoCreation]
    public abstract class ComponentDataSerializationSystem<T> : JobComponentSystem where T : struct,IComponentData
    {
        public WorldSerializationSystemManager SerializationManger;
        /// <summary>
        /// 该系统依赖信息
        /// </summary>
        private EntityQuery eq;
        protected override void OnCreate()
        {
            base.OnCreate();
            SerializationManger = World.Active.GetExistingSystem<WorldSerializationSystemManager>();
            eq = GetEntityQuery(ComponentType.ReadOnly<Identification>(),ComponentType.ReadOnly<T>());
            eq.SetFilterChanged(ComponentType.ReadOnly<T>());
        }
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            //用过滤器留下需要保存的场景的组件
            DataFilter filter = new DataFilter
            {
                identifications = eq.ToComponentDataArray<Identification>(Allocator.TempJob),
                datas = eq.ToComponentDataArray<T>(Allocator.TempJob),
                Scenes = new NativeArray<int>(SerializationManger.ToBeSerializedSceneIds, Allocator.TempJob),
                eachscenedata = new NativeArray<NativeList<(Identification, T)>>
            (SerializationManger.ToBeSerializedSceneIds.Length, Allocator.TempJob)
            };
            for (int i = 0; i < filter.Scenes.Length; i++)
            {
                filter.eachscenedata[i] = new NativeList<(Identification, T)>(Allocator.TempJob);
            }
            var Handle = filter.Schedule(inputDependencies);
            for(int i = 0; i < filter.Scenes.Length; i++)
            {
                Handle = Serialize(Handle, filter.eachscenedata[i]);
            }
            return Handle;
        }
        /// <summary>
        /// 组件数据过滤器
        /// </summary>
        struct DataFilter : IJob
        {
            public NativeArray<Identification> identifications;
            public NativeArray<T> datas;
            public NativeArray<int> Scenes;
            public NativeArray<NativeList<(Identification, T)>> eachscenedata;
            public void Execute()
            {
                for (int i = 0; i < identifications.Length; i++)
                {
                    for (int j = 0; j < Scenes.Length; j++)
                    {
                        //如果该组件所属实体是要存的某个场景的实体之一，将其添加到对应场景的组件列表中
                        if(identifications[i].SceneId == Scenes[j])
                        {
                            eachscenedata[j].Add((identifications[i], datas[i]));
                        }
                    }
                }
                //筛选完后把筛选用数组释放掉
                identifications.Dispose();
                datas.Dispose();
                Scenes.Dispose();
                eachscenedata.Dispose();
            }
        }
        protected abstract JobHandle Serialize(JobHandle Denpendences,NativeList<(Identification,T)> datas);
        /// <summary>
        /// 该类所序列化的组件的名称
        /// </summary>
        public virtual string ComponentDataTypeName { get { return ComponentDataType.FullName; } }
        /// <summary>
        /// 该类所序列化的组件的类型
        /// </summary>
        public virtual System.Type ComponentDataType { get { return typeof(T); } }
    }

}