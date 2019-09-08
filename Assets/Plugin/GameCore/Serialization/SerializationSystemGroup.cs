using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using GameCore.Serialization;
using Unity.Collections;
using Unity.Jobs;
namespace GameCore
{
    [DisableAutoCreation]
    public class SerializationSystemGroup : ComponentSystemGroup
    {
        
    }
    [DisableAutoCreation]
    public class WorldSerializationManager : ComponentSystemGroup
    {
        
        /// <summary>
        /// 系统组所属存档
        /// </summary>
        public SaveManager Save;
        public DataStoreSystem StoreSystem;
        //以下为在一轮序列化中使用的变量
        /// <summary>
        /// 该轮保存要保存的场景
        /// </summary>
        public int[] ToBeSerializedSceneIds;
        /// <summary>
        /// 该轮所有序列化任务的句柄
        /// </summary>
        public NativeList<JobHandle> SerializationHandles;
        /// <summary>
        /// 该轮序列化的数据保存句柄
        /// </summary>
        public JobHandle StoreHandle;
        /// <summary>
        /// 该轮序列化的总句柄
        /// </summary>
        public JobHandle MainSerializationHandle;
        public bool IsSaving { get => StoreSystem.IsSaving; }

        protected override void OnCreate()
        {
            base.OnCreate();
            Save = SaveManager.Active;
            //添加存档中所有mod的序列化系统
            var mods = Save.Mods;
            for (int i = 0; i < mods.Count; i++)
            {
                AddSystemToUpdateList(mods[i].GetSerializationSystemGroup());
            }
            SortSystemUpdateList();

            StoreSystem = Save.SystemsManager.StoreSystem;
        }
        /// <summary>
        /// 保存场景数据
        /// </summary>
        /// <param name="SceneIds">要序列化的所有场景id</param>
        public void Serialize(int[] SceneIds)
        {
            //初始化该轮序列化使用变量
            ToBeSerializedSceneIds = SceneIds;
            SerializationHandles = new NativeList<JobHandle>(Allocator.Temp);
            Update();
            //合并出初步总序列化句柄给StoreSystem使用
            MainSerializationHandle = JobHandle.CombineDependencies(SerializationHandles);
            //此时序列化任务句柄列表就不需要了
            SerializationHandles.Dispose();
            StoreHandle = StoreSystem.Save(MainSerializationHandle);
            //合并出最终总序列化句柄给其它需要监听序列化任务的类使用
            MainSerializationHandle = StoreHandle;
        }
    }
}
