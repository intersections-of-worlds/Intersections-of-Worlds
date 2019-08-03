using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using System;
using System.Reflection;

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
        /// 该场景会生成的地形列表
        /// </summary>
        public List<string> Terrains;
        public void Creat(RandomSeed seed)
        {
            SceneMap map = new SceneMap(length, width);

        }
        private void CreatTerrains(ref SceneMap map,RandomSeed seed)
        {
            //挨个创建
            for (int i = 0; i < Terrains.Count; i++)
            {
                var t = SaveManager.Active.Get<SceneTerrain>(Terrains[i]);
                ITerrainCreator tc;
                try
                {
                    tc = (ITerrainCreator)Activator.CreateInstance(Type.GetType(t.TerrainCreatorName));
                }
                catch
                {
                    throw new ModAssetException("无法获取地形创建类", t.GetAssetModName(), t.GetAssetName());
                }
                tc.Creat(ref map, seed.Add(t.TerrainCreatorName.GetHash()));

            }
        }
        private void CreatBuildings(ref SceneMap map, RandomSeed seed)
        {
            List<SceneBuilding> ts = SaveManager.Active.GetAll<SceneBuilding>();
        }

    }
}
