using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
namespace GameCore
{
    public class SaveSystemsManager : IDisposable
    {
        public WorldUpdateGroup UpdateGroup;
        public WorldObjectManager ObjectManager;
        public WorldSerializationSystemManager SerializationManager;
        SaveManager Save;
        public SaveSystemsManager(SaveManager save)
        {
            Save = save;
            UpdateGroup = World.Active.CreateSystem<WorldUpdateGroup>();
            UpdateGroup.SetSave(save);
            World.Active.GetExistingSystem<SimulationSystemGroup>().AddSystemToUpdateList(UpdateGroup);
            ObjectManager = World.Active.CreateSystem<WorldObjectManager>();
            SerializationManager = World.Active.CreateSystem<WorldSerializationSystemManager>();
            SerializationManager.SetSave(save);
        }

        public void Dispose()
        {
            World.Active.GetExistingSystem<SimulationSystemGroup>().RemoveSystemFromUpdateList(UpdateGroup);
            World.Active.DestroySystem(UpdateGroup);

        }
    }
}
