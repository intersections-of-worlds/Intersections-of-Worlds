using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TestConverter : MonoBehaviour,IConvertGameObjectToEntity
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Convert(Entity entity,EntityManager dstmanager,GameObjectConversionSystem system){
        system.DeclareLinkedEntityGroup(this.gameObject);
    }
}
