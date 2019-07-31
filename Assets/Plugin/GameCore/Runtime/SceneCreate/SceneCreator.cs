using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using System;

namespace GameCore
{
    [Serializable]
    [CreateAssetMenu(menuName = "Intersections of Worlds/Scene/SceneCreator")]
    public class SceneCreator : ScriptableObject
    {
        public int length;
        public int width;
        public string SceneCreatorName;
        /// <summary>
        /// 该场景会生成的地形的名称
        /// </summary>
        public List<string> Terrains;
        public void Creat(RandomSeed seed)
        {
            SceneMap map = new SceneMap(length, width);
        }
        private void CreatTerrains(ref SceneMap map,RandomSeed seed)
        {

        }
        private void CreatBuildings(ref SceneMap map, RandomSeed seed)
        {

        }

    }
}
