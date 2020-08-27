using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using DragonBones;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Burst;
using System.IO;
using Unity.Transforms;
using Unity.Mathematics;


public class PlayerMoveSystem : ComponentSystem
{
    EntityQuery eq;
    protected override void OnCreate()
    {
        base.OnCreate();
        eq = GetEntityQuery(new ComponentType[]{ new ComponentType(typeof(UnityArmatureComponent)),
            new ComponentType(typeof(Translation)),ComponentType.ReadWrite<NonUniformScale>()});
        RequireForUpdate(eq);
    }
    protected override void OnUpdate()
    {
        var playerArmatures = eq.ToComponentArray<UnityArmatureComponent>();
        var playerTransforms = eq.ToComponentDataArray<Translation>(Unity.Collections.Allocator.TempJob);
        var playerScales = eq.ToComponentDataArray<NonUniformScale>(Unity.Collections.Allocator.TempJob);
        var playerTransform = playerTransforms[0];
        var playerScale = playerScales[0];
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (!playerArmatures[0].animation.isPlaying)
            {
                playerArmatures[0].animation.Play("walk");
            }
        }
        else
        {
            playerArmatures[0].animation.Play("walk");
            playerArmatures[0].animation.Stop();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            playerScale.Value = new float3(1, 1, 1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            playerScale.Value = new Vector3(-1, 1, 1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("A按下");
            playerTransform.Value = new Vector3(playerTransform.Value.x - 3f * Time.DeltaTime, playerTransform.Value.y, 1);

        }
        if (Input.GetKey(KeyCode.D))
        {
            Debug.Log("D按下");
            playerTransform.Value = new Vector3(playerTransform.Value.x + 3f * Time.DeltaTime, playerTransform.Value.y, 1);

        }
        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("W按下");
            playerTransform.Value = new Vector3(playerTransform.Value.x, playerTransform.Value.y + 3f * Time.DeltaTime, 1);

        }
        if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("D按下");
            playerTransform.Value = new Vector3(playerTransform.Value.x, playerTransform.Value.y - 3f * Time.DeltaTime, 1);

        }
        //存一下
        playerTransforms[0] = playerTransform;

        playerScales[0] = playerScale;
        eq.CopyFromComponentDataArray(playerTransforms);
        eq.CopyFromComponentDataArray(playerScales);
        playerTransforms.Dispose();
        playerScales.Dispose();
    }
}
