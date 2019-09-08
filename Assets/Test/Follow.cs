using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform TargetTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = (Unity.Mathematics.float3)TargetTransform.position - new Unity.Mathematics.float3(0,0,28);
    }
}
