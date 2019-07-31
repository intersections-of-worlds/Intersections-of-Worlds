using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using DragonBones;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Burst;
using System.IO;


public class PlayerMoveSystem : ComponentSystem
{
    EntityQuery eq;
    protected override void OnCreate()
    {
        eq = GetEntityQuery(new ComponentType[]{ new ComponentType(typeof(UnityArmatureComponent)),
            new ComponentType(typeof(UnityEngine.Transform))});
        RequireForUpdate(eq);
    }
    protected override void OnUpdate()
    {
        
        var playerArmatures = eq.ToComponentArray<UnityArmatureComponent>();
        var playerTransforms = eq.ToComponentArray<UnityEngine.Transform>();
        Debug.Log("运行中");
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
            playerTransforms[0].localScale = new Vector3(1, 1, 1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            playerTransforms[0].localScale = new Vector3(-1, 1, 1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("A按下");
            playerTransforms[0].localPosition = new Vector3(playerTransforms[0].position.x - 5 * Time.deltaTime, playerTransforms[0].position.y, 0);

        }
        if (Input.GetKey(KeyCode.D))
        {
            Debug.Log("D按下");
            playerTransforms[0].localPosition = new Vector3(playerTransforms[0].position.x + 5 * Time.deltaTime, playerTransforms[0].position.y, 0);

        }
        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("W按下");
            playerTransforms[0].localPosition = new Vector3(playerTransforms[0].position.x, playerTransforms[0].position.y + 5 * Time.deltaTime, 0);

        }
        if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("D按下");
            playerTransforms[0].localPosition = new Vector3(playerTransforms[0].position.x, playerTransforms[0].position.y - 5 * Time.deltaTime, 0);

        }

    }
}
