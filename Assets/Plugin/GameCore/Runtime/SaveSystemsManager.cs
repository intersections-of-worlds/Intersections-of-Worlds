using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
namespace GameCore
{
    public class SaveSystemsManager : IDisposable
    {
        WorldUpdateGroup UpdateGroup;
        SaveManager Save;
        public SaveSystemsManager(SaveManager save)
        {
            Save = save;
            UpdateGroup = World.Active.CreateSystem<WorldUpdateGroup>(save);
            World.Active.GetExistingSystem<SimulationSystemGroup>().AddSystemToUpdateList(UpdateGroup);
        }

        public void Dispose()
        {
            World.Active.GetExistingSystem<SimulationSystemGroup>().RemoveSystemFromUpdateList(UpdateGroup);
            World.Active.DestroySystem(UpdateGroup);
        }
    }
}
