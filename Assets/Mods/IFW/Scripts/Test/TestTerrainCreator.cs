using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore;
using Unity.Mathematics;
namespace IFW
{
    public class TestTerrainCreator : ITerrainCreator
    {
        public void Create(ISceneMap Map, RandomSeed seed)
        {
            for(int3 i = new int3(0, 0, 0); i.x < Map.Size.x; i.x++)
            {
                for (i.y = 0; i.y < Map.Size.y; i.y++)
                {
                    Map.SetTile(i, SaveManager.Active.GetRef("IFW.TestTile"));
                }
            }
        }
    }
    public class TestTerrainCreator2 : ITerrainCreator
    {
        public void Create(ISceneMap Map, RandomSeed seed)
        {
            for (int3 i = new int3(0, 0, 0); i.x < Map.Size.x; i.x++)
            {
                for (i.y = 0; i.y < Map.Size.y; i.y++)
                {
                    Map.SetTile(i, SaveManager.Active.GetRef("IFW.TestTile2"));
                }
            }
        }
    }
}
