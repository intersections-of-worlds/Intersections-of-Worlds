using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
namespace GameCore
{
    [DisableAutoCreation]
    public class SerializationSystemGroup : ComponentSystemGroup
    {

    }
    [DisableAutoCreation]
    public class WorldSerializationSystemManager : ComponentSystemGroup
    {
        /// <summary>
        /// 该轮保存要保存的场景
        /// </summary>
        public int[] ToBeSerializedSceneIds;
        public WorldSerializationSystemManager(SaveManager save)
        {
            Save = save;
        }
        /// <summary>
        /// 系统组所属存档
        /// </summary>
        public SaveManager Save;
        protected override void OnCreate()
        {
            base.OnCreate();
            
        }
        public void SetSave(SaveManager save)
        {
            Save = save;
            var mods = Save.Mods;
            for (int i = 0; i < mods.Count; i++)
            {
                AddSystemToUpdateList(mods[i].GetUpdateSystemGroup());
            }
            SortSystemUpdateList();
        }
        /// <summary>
        /// 保存场景数据
        /// </summary>
        /// <param name="SceneIds">要序列化的所有场景id</param>
        public void Serialize(int[] SceneIds)
        {
            ToBeSerializedSceneIds = SceneIds;
            Update();
        }
    }
}
