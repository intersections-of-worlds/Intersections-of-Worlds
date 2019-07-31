using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
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
        Debug.Log("创建成功");
    }
    protected override void OnUpdate()
    {
        Debug.Log(Id + "TestSystem运行中" + Time.time);
    }
}
[DisableAutoCreation]
public class MyComponentSystemGroup : ComponentSystemGroup {
    protected override void OnCreate()
    {

        base.OnCreate();
        this.AddSystemToUpdateList(World.CreateSystem<TestSystem>(2));
        AddSystemToUpdateList(World.CreateSystem<TestSystem>(1));
    }
}
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class InitSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        base.OnCreate();
        string b = "Python牛逼";
        string a = "C#牛逼";
        Debug.Log(a.GetHashCode());
        MyJob m = new MyJob();
        m.Schedule();
    }
    protected override void OnUpdate()
    {
    }
}
public struct MyJob : MyInteface
{
    public NativeArray<int> myarray { get; set; }
    public void Execute()
    {
        
    }
}
public interface MyInteface : IJob
{
    NativeArray<int> myarray { get; set; }
}
