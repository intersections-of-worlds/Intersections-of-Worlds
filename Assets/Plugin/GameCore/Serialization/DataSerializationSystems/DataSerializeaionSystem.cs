using Unity.Entities;
using Unity.Jobs;

namespace GameCore
{
    public abstract class DataSerializeaionSystem<T> : JobComponentSystem where T : struct
    {
        public WorldSerializationManager SerializationManger;
        /// <summary>
        /// 该系统依赖信息
        /// </summary>
        protected EntityQuery eq;
        
        /// <summary>
        /// 该类所序列化的组件的名称
        /// </summary>
        public string ComponentDataTypeName = typeof(T).FullName;
        /// <summary>
        /// 该类所序列化的组件的类型
        /// </summary>
        public virtual System.Type ComponentDataType { get { return typeof(T); } }
        protected override void OnCreate()
        {
            base.OnCreate();
            SerializationManger = World.Active.GetExistingSystem<WorldSerializationManager>();
            eq = GetEntityQuery(ComponentType.ReadOnly<Identification>(), ComponentType.ReadOnly<T>());
            if (SerializationManger.StoreSystem.EnableSetFilterChange)
            {
                eq.SetFilterChanged(ComponentType.ReadOnly<T>());
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            JobHandle jobHandle = OnUpdateInternal(inputDeps);
            //将序列化句柄添加到总序列化句柄中
            SerializationManger.SerializationHandles.Add(jobHandle);
            return jobHandle;
        }
        protected abstract JobHandle OnUpdateInternal(JobHandle inputDependencies);
    }

}