using Unity.Entities;
using Unity.Collections;
using GameCore;
using Unity.Jobs;
using System.Collections.Generic;

namespace GameCore.Serialization
{
    /// <summary>
    /// 负责储存数据的类
    /// </summary>
    public abstract class DataStoreSystem : ComponentSystem
    {
        public SaveManager save;
        /// <summary>
        /// 是否过滤未更改的组件
        /// </summary>
        public abstract bool EnableSetFilterChange { get; }
        /// <summary>
        /// 获得当前场景的储存器
        /// </summary>
        public abstract NativeQueue<SerializedComponentData>.ParallelWriter GetStorer(int SceneId);
        public abstract bool IsSaving { get; }
        public DataStoreSystem()
        {
            save = SaveManager.Active;
        }
        public virtual JobHandle Save(JobHandle inputDeps)
        {
            if (IsSaving)
            {
                throw new System.Exception("正在序列化中，请勿在此时序列化！");
            }
            JobHandle handle = SaveInternal(inputDeps);
            return handle;
        }
        public virtual JobHandle Read(int SceneId,out NativeArray<EntitySerializedData> result)
        {
            return ReadInternal(SceneId,out result);
        }
        protected abstract JobHandle ReadInternal(int SceneId, out NativeArray<EntitySerializedData> result);
        protected abstract JobHandle SaveInternal(JobHandle inputDeps);
    }
    public struct SerializedComponentData : System.IDisposable
    {
        public Identification Identification;
        public NativeArray<byte> ComponentTypeName;
        public NativeArray<byte> ComponentData;
        public void SetIdentification(Identification identification)
        {
            Identification = identification;
        }
        public void SetComponetTypeName(string TypeName)
        {
            //如果已有值，先把之前的值释放
            if (ComponentTypeName.IsCreated)
                ComponentTypeName.Dispose();
            ComponentTypeName = new NativeArray<byte>(DefaultSerializer.ToByte(TypeName), Allocator.TempJob);
        }
        public string GetComponentTypeName()
        {
            return DefaultSerializer.FromByte(ComponentTypeName.ToArray());
        }
        public void SetComponentData(byte[] componentData)
        {
            //如果已有值，先把之前的值释放
            if (ComponentData.IsCreated)
                ComponentData.Dispose();
            ComponentData = new NativeArray<byte>(componentData, Allocator.TempJob);
        }
        public byte[] GetComponentData()
        {
            return ComponentData.ToArray();
        }
        public void Dispose()
        {
            ComponentTypeName.Dispose();
            ComponentData.Dispose();
        }
        public struct ManagedType
        {
            public Identification Identification;
            public string ComponentTypeName;
            public byte[] ComponentData;
        }
        public ManagedType GetManagedType()
        {
            return new ManagedType
            {
                Identification = Identification,
                ComponentTypeName = GetComponentTypeName(),
                ComponentData = GetComponentData()
            };
        }
    }
    /// <summary>
    /// Entity的所有组件的数据容器 
    /// </summary>
    public struct EntitySerializedData : System.IDisposable
    {
        /// <summary>
        /// 实体的标识
        /// </summary>
        public Identification Identification;
        /// <summary>
        /// 实体组件的数据，key为组件名，value为组件数据
        /// </summary>
        public NativeHashMap<NativeArray<byte>, NativeArray<byte>> Components;
        public EntitySerializedData(SerializedComponentData componentData)
        {
            Identification = componentData.Identification;
            Components = new NativeHashMap<NativeArray<byte>, NativeArray<byte>>(2, Allocator.TempJob);
            Components.TryAdd(componentData.ComponentTypeName, componentData.ComponentData);
        }
        public struct ManagedType
        {
            public Identification Identification;
            public Dictionary<string, byte[]> Components;
        }
        public ManagedType GetManagedType()
        {
            Dictionary<string, byte[]> components = new Dictionary<string, byte[]>();
            using (var keys = Components.GetKeyArray(Allocator.Temp))
            {
                for(int i = 0; i < keys.Length; i++)
                {
                    components.Add(DefaultSerializer.FromByte(keys[i].ToArray()), Components[keys[i]].ToArray());
                }
            }
            return new ManagedType
            {
                Identification = Identification,
                Components = components
            };
        }
        public void Dispose()
        {
            using (var keys = Components.GetKeyArray(Allocator.Temp))
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    Components[keys[i]].Dispose();
                    keys[i].Dispose();
                }
            }
            Components.Dispose();
        }
    }

}