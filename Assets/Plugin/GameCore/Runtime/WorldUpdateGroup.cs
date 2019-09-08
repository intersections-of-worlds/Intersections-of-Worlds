using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace GameCore
{
    /// <summary>
    /// 游戏帧刷新系统组
    /// </summary>
    public class WorldUpdateGroup : ComponentSystemGroup
    {
        public WorldUpdateGroup()
        {
        }
        /// <summary>
        /// 系统组所属存档
        /// </summary>
        public SaveManager Save;
        public virtual void SetSave(SaveManager save)
        {
            Save = save;

            var mods = Save.Mods;
            for (int i = 0; i < mods.Count; i++)
            {
                AddSystemToUpdateList(mods[i].GetUpdateSystemGroup());
            }
            SortSystemUpdateList();
        }
        protected override void OnCreate()
        {
            base.OnCreate();
        }
    }
}

