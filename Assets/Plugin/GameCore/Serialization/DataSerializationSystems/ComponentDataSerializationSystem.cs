using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using GameCore.Serialization;

namespace GameCore
{
    public abstract partial class ComponentDataSerializationSystem<T> : DataSerializeaionSystem<T> where T : struct,IComponentData
    {
        
        protected override JobHandle OnUpdateInternal(JobHandle inputDependencies)
        {
            //用过滤器留下需要保存的场景的组件
            DataFilter<T> filter = new DataFilter<T>
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
            //筛选器的句柄
            var FilterHandle = filter.Schedule(inputDependencies);
            NativeArray<JobHandle> Handles = new NativeArray<JobHandle>(filter.Scenes.Length,Allocator.Temp);
            for(int i = 0; i < filter.Scenes.Length; i++)
            {
                Handles[i] = Serialize(FilterHandle, SerializationManger.StoreSystem.GetStorer(filter.Scenes[i])
                    ,filter.eachscenedata[i]);
            }
            //将所有场景的序列化job的句柄合并
            var Handle = JobHandle.CombineDependencies(Handles);
            Handles.Dispose();
            return Handle;
        }
        /// <summary>
        /// 场景组件序列化函数
        /// </summary>
        /// <param name="Denpendences">依赖的job</param>
        /// <param name="Storer">序列化后的数据的储存器</param>
        /// <param name="datas">要序列化的数据</param>
        /// <returns>序列化job的句柄</returns>
        protected virtual JobHandle Serialize(JobHandle Denpendences, 
            NativeQueue<SerializedComponentData>.ParallelWriter Storer, NativeList<(Identification,T)> datas)
        {
            SerializeJob job = new SerializeJob
            {
                componentTypeName = new NativeArray<byte>(DefaultSerializer.ToByte(ComponentDataTypeName), Allocator.TempJob),
                Storer = Storer,
                datas = datas
            };
            return job.Schedule(datas.Length, 64, Denpendences);
        }
        /// <summary>
        /// 用于序列化组件的Job
        /// </summary>
        protected struct SerializeJob : IJobParallelFor
        {
            /// <summary>
            /// 组件名
            /// </summary>
            [DeallocateOnJobCompletion]
            public NativeArray<byte> componentTypeName;
            /// <summary>
            /// 组件数据储存器
            /// </summary>
            public NativeQueue<SerializedComponentData>.ParallelWriter Storer;
            /// <summary>
            /// 组件数据列表
            /// </summary>
            [DeallocateOnJobCompletion]
            public NativeList<(Identification, T)> datas;
            public void Execute(int index)
            {
                SerializedComponentData serializedComponentData = new SerializedComponentData();
                //设置组件所属实体标识
                serializedComponentData.SetIdentification(datas[index].Item1);
                //设置组件名
                componentTypeName.CopyTo(serializedComponentData.ComponentTypeName);
                //设置组件序列化后数据
                serializedComponentData.SetComponentData(DefaultSerializer.Serialize(datas[index].Item2));
                Storer.Enqueue(serializedComponentData);
            }
        }
    }

}