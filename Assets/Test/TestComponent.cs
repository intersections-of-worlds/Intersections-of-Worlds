using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using GameCore;

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
    public GameObject Prefab;
    public int data;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<TestECSComponent>(entity,new TestECSComponent(data));
    }
    public void Start()
    {
        
    }
    void Update()
    {
        if (data == 0)
        {
            //World.Active.EntityManager.Instantiate(World.Active.GetExistingSystem<GameObjectConversionSystem>().GetPrimaryEntity(Prefab));
            //Instantiate(Prefab);
            Entity e = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab,World.Active);
            //World.Active.EntityManager.AddComponentData(e, new Unity.Transforms.NonUniformScale());
            //World.Active.EntityManager.GetComponentObject<Transform>(e);
            World.Active.EntityManager.AddComponentData(e, new Unity.Transforms.NonUniformScale());
            World.Active.EntityManager.RemoveComponent<Unity.Transforms.Parent>(e);
            World.Active.EntityManager.RemoveComponent<Unity.Transforms.LocalToParent>(e);
            Debug.Log("已生成");
            //World.Active.EntityManager.DestroyEntity(Prefab.GetComponent<WorldObject>().entity);
        }
        data += 1;
    }
}
