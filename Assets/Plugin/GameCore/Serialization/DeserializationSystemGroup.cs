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
    [DisableAutoCreation]
    public class DeserializationSystemGroup : ComponentSystemGroup
    {

    }
    public class WorldDeserializationManager : ComponentSystemGroup
    {
        /// <summary>
        /// 系统组所属存档
        /// </summary>
        public SaveManager Save;
        public DataStoreSystem StoreSystem;
        protected override void OnCreate()
        {
            base.OnCreate();
            Save = SaveManager.Active;
            //添加存档中所有mod的序列化系统
            var mods = Save.Mods;
            for (int i = 0; i < mods.Count; i++)
            {
                AddSystemToUpdateList(mods[i].GetDeserializationSystemGroup());
            }
            SortSystemUpdateList();

            StoreSystem = Save.SystemsManager.StoreSystem;
        }
        /// <summary>
        /// 保存场景数据
        /// </summary>
        /// <param name="SceneIds">要序列化的所有场景id</param>
        public void Deserialize(int SceneId)
        {

        }
    }
}
