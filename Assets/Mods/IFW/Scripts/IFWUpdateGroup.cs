using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using GameCore;
namespace IFW
{
    [DisableAutoCreation]
    public class IFWUpdateGroup : ComponentSystemGroup
    {
        public IFWUpdateGroup()
        {

        }
    }
    [UpdateInGroup(typeof(IFWUpdateGroup))]
    public class MyUpdateGroup : ComponentSystem
    {
        private int me = 0;
        protected override void OnCreate()
        {
            base.OnCreate();
            
        }
        protected override void OnUpdate()
        {
            
        }
    }
}
