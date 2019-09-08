using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GameCore.Serialization
{
    [DisableAutoCreation]
    public class FileStoreSystem : DataStoreSystem
    {
        #region DataPath
        public string GetSceneDataPath(int SceneId)
        {
            return save.DataPath + "/" + SceneId + ".dat";
        }
        #endregion

        public override bool EnableSetFilterChange => false;

        public override bool IsSaving { get => SavingScenesCount == 0; }

        protected NativeHashMap<int,NativeQueue<SerializedComponentData>> scenesToSave;
        /// <summary>
        /// 正在将数据转化为最后的二进制数据的Job，int为场景id
        /// </summary>
        protected List<(int, JobHandle,NativeArray<byte>)> SerializingJobs = new List<(int, JobHandle,NativeArray<byte>)>();
        protected List<(Task, FileStream)> SavingTasks = new List<(Task, FileStream)>();
        protected int SavingScenesCount = 0;

        public override NativeQueue<SerializedComponentData>.ParallelWriter GetStorer(int SceneId)
        {
            
            if (!scenesToSave.IsCreated)
            {
                //如果已存在就返回已有的目标场景数据储存器，如果不存在就创建一个新的
                if (scenesToSave.ContainsKey(SceneId))
                {
                    return scenesToSave[SceneId].AsParallelWriter() ;
                }
                else
                {
                    NativeQueue<SerializedComponentData> result = new NativeQueue<SerializedComponentData>(Allocator.TempJob);
                    scenesToSave.TryAdd(SceneId,result);
                    return result.AsParallelWriter();
                }
            }
            else
            {
                //如果该映射表没有创建，将其创建并添加新的数据储存器到其中
                scenesToSave = new NativeHashMap<int, NativeQueue<SerializedComponentData>>(2, Allocator.TempJob);
                NativeQueue<SerializedComponentData> result = new NativeQueue<SerializedComponentData>(Allocator.TempJob);
                scenesToSave.TryAdd(SceneId,result);
                return result.AsParallelWriter();
            }
        }

        protected override JobHandle SaveInternal(JobHandle inputDeps)
        {
            NativeArray<int> Scenes = scenesToSave.GetKeyArray(Allocator.Temp);
            SavingScenesCount = Scenes.Length;
            NativeArray<JobHandle> Handles = new NativeArray<JobHandle>(SavingScenesCount, Allocator.Temp);
            for (int i = 0; i < SavingScenesCount; i++)
            {
                ClassifyJob c = new ClassifyJob { ComponentDatas = scenesToSave[Scenes[i]] };
                var CHandle = c.Schedule(inputDeps);
                SerializeJob s = new SerializeJob { datas = c.Result };
                var SHandle = s.Schedule(CHandle);
                Handles[i] = SHandle;
                SerializingJobs.Add((Scenes[i], SHandle,s.result));
            }
            Scenes.Dispose();
            var result = JobHandle.CombineDependencies(Handles);
            Handles.Dispose();
            scenesToSave.Dispose();
            return result;
        }

        protected override void OnUpdate()
        {
            if (!IsSaving)
                return;
            //因为在迭代中进行删除操作会出现问题，所以将删除操作延后
            NativeList<int> operaterBuffer = new NativeList<int>(Allocator.Temp);
            //当二进制数据转换job完成后将二进制数据储存
            for(int i = 0; i < SerializingJobs.Count; i++)
            {
                if (SerializingJobs[i].Item2.IsCompleted)
                {
                    SerializingJobs[i].Item2.Complete();
                    operaterBuffer.Add(i);
                }
            }
            for(int i = 0;i< operaterBuffer.Length; i++)
            {
                int index = operaterBuffer[i];
                FileStream fs = File.Open(GetSceneDataPath(SerializingJobs[index].Item1),FileMode.OpenOrCreate);
                Task t = fs.WriteAsync(SerializingJobs[index].Item3.ToArray(),0, SerializingJobs[index].Item3.Length);
                SavingTasks.Add((t, fs));
                SerializingJobs[index].Item3.Dispose();
                SerializingJobs.RemoveAt(index);
            }
            operaterBuffer.Clear();
            //检查保存任务，如果有完成的释放filestream并将保存任务总数减一
            for(int i = 0; i < SavingTasks.Count; i++)
            {
                if (SavingTasks[i].Item1.IsCompleted)
                {
                    operaterBuffer.Add(i);
                }
            }
            for (int i = 0; i < operaterBuffer.Length; i++)
            {
                int index = operaterBuffer[i];
                SavingScenesCount -= 1;
                SavingTasks[index].Item2.Dispose();
                SavingTasks.RemoveAt(index);
            }
        }

        protected override JobHandle ReadInternal(int SceneId, out NativeArray<EntitySerializedData> result)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 将组件数据加入到其对应的实体的序列化器上
        /// </summary>
        struct ClassifyJob : IJob
        {
            /// <summary>
            /// 要进行分类的组件数据
            /// </summary>
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeQueue<SerializedComponentData> ComponentDatas;
            public NativeArray<EntitySerializedData> Result;
            public void Execute()
            {
                NativeHashMap<Identification, EntitySerializedData> result = new NativeHashMap<Identification, EntitySerializedData>(10,Allocator.Temp);
                SerializedComponentData item;
                while (ComponentDatas.TryDequeue(out item))
                {
                    //有则添加，没有则创建
                    if (result.ContainsKey(item.Identification))
                    {
                        result[item.Identification].Components.TryAdd(item.ComponentTypeName,item.ComponentData);
                    }
                    else
                    {
                        result.TryAdd(item.Identification, new EntitySerializedData(item));//数据添加在这个构造函数中已经完成了
                    }
                }
                Result = result.GetValueArray(Allocator.TempJob);
                result.Dispose();
            }
        }
        /// <summary>
        /// 将所有EntitySerializedData转换成二进制数据的job
        /// </summary>
        struct SerializeJob : IJob
        {
            [DeallocateOnJobCompletion]
            public NativeArray<EntitySerializedData> datas;
            public NativeArray<byte> result;
            public void Execute()
            {
                //将数据转化成托管类型后序列化
                EntitySerializedData.ManagedType[] managedDatas = new EntitySerializedData.ManagedType[datas.Length];
                for(int i = 0; i < datas.Length; i++)
                {
                    managedDatas[i] = datas[i].GetManagedType();
                    datas[i].Dispose();
                }
                result = new NativeArray<byte>(DefaultSerializer.Serialize(managedDatas), Allocator.TempJob);
            }
        }
    }

}