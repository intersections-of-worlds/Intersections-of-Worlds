using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
using GameCore.Serialization;
namespace GameCore
{
    public class SaveSystemsManager : IDisposable
    {
        public WorldUpdateGroup UpdateGroup;
        public WorldObjectManager ObjectManager;
        public DataStoreSystem StoreSystem;
        public WorldSerializationManager SerializationManager;
        public WorldDeserializationManager DeserializationManager;
        public PrefabInstantiator Instantiator;
        SaveManager Save;
        public SaveSystemsManager()
        {
            
        }
        public void Init(SaveManager save)
        {
            Save = save;
            Instantiator = World.Active.CreateSystem<PrefabInstantiator>();
            UpdateGroup = World.Active.CreateSystem<WorldUpdateGroup>();
            StoreSystem = World.Active.CreateSystem<FileStoreSystem>();
            var simGroup = World.Active.GetExistingSystem<SimulationSystemGroup>();
            simGroup.AddSystemToUpdateList(UpdateGroup);
            simGroup.AddSystemToUpdateList(Instantiator);
            simGroup.AddSystemToUpdateList(StoreSystem);
            ObjectManager = World.Active.CreateSystem<WorldObjectManager>();
            SerializationManager = World.Active.CreateSystem<WorldSerializationManager>();
            DeserializationManager = World.Active.CreateSystem<WorldDeserializationManager>();
        }
        public void Dispose()
        {
            World.Active.GetExistingSystem<SimulationSystemGroup>().RemoveSystemFromUpdateList(UpdateGroup);
            World.Active.DestroySystem(UpdateGroup);
            World.Active.DestroySystem(StoreSystem);
            World.Active.DestroySystem(SerializationManager);
            World.Active.DestroySystem(DeserializationManager);
        }
    }
}
