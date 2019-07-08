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
    public NativeString64 a;
}
[AddComponentMenu("Test/TestComponent")]
public class TestComponent : ComponentDataProxy<TestECSComponent>
{
}
