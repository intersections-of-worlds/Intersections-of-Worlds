using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

[System.Serializable]
public struct TestECSComponent : IComponentData
{
    public int data;

    public TestECSComponent(int data)
    {
        this.data = data;
    }
}
[AddComponentMenu("Test/TestComponent")]
public class TestComponent : MonoBehaviour ,IConvertGameObjectToEntity
{
    public int data;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<TestECSComponent>(entity,new TestECSComponent(data));
    }
    void Update()
    {
        Debug.Log("我还活着");
    }
}
