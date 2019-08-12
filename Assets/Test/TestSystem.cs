using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using GameCore;
[UpdateInGroup(typeof(MyComponentSystemGroup))]
public class TestSystem : ComponentSystem
{
    public int Id = 0;
    public TestSystem()
    {
        
    }
    public TestSystem(int id)
    {
        Id = id;
    }
    protected override void OnCreate()
    {
        base.OnCreate();

    }
    protected override void OnUpdate()
    {
        
    }
}
[DisableAutoCreation]
public class MyComponentSystemGroup : ComponentSystemGroup {
    private SaveManager save;
    protected override void OnCreate()
    {
        base.OnCreate();
        GameString.Init(Language.Chinese);
        SaveManager save = SaveManager.CreateSave("TestSave");
        save.Load();
    }
}