using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using System;
using System.Reflection;
using Unity.Mathematics;

namespace GameCore
{
    [Serializable]
    [CreateAssetMenu(menuName = "Intersections of Worlds/Scene/SceneCreator")]
    public class SceneCreator : ScriptableObject
    {
        /// <summary>
        /// 找生成位置的尝试次数
        /// </summary>
        public const int TryTimes = 5;
        public const int MaxLength = 100;
        public const int MaxWidth = 100;
        [Range(0,MaxLength)]
        public int length;
        [Range(0,MaxWidth)]
        public int width;
        //public string SceneCreatorName = "defalut";
        /// <summary>
        /// 该场景会生成的地形列表
        /// </summary>
        public List<string> Terrains;
        public void Create(SceneMap map,RandomSeed seed)
        {
            CreateTerrains(map, seed.Add("Terrains".GetHash()));
            CreateBuildings(map, seed.Add("Buildings".GetHash()));
            CreateBiomes(map, seed.Add("Biomes".GetHash()));
        }
        public SceneMap CreateSceneMap(SceneManager scene)
        {
            return new SceneMap(scene, length, width);
        }
        private void CreateTerrains(SceneMap map,RandomSeed seed)
        {
            //挨个创建
            for (int i = 0; i < Terrains.Count; i++)
            {
                var t = SaveManager.Active.Get<SceneTerrain>(Terrains[i]);
                ITerrainCreator tc;
                try
                {
                    tc = (ITerrainCreator)Activator.CreateInstance(Assembly.GetExecutingAssembly().GetType(t.TerrainCreatorName));
                }
                catch
                {
                    Debug.Log("获取" + t.TerrainCreatorName + "失败");
                    throw new ModAssetException("无法获取地形创建类", t.GetAssetModName(), t.GetAssetName());
                }
                tc.Create(map, seed.Add(t.TerrainName.GetHash()));

            }
        }
        private void CreateBuildings(SceneMap map, RandomSeed seed)
        {
            List<SceneBuilding> bs = MatchBuildings(Terrains);
            //挨个匹配
            for(int i = 0; i < bs.Count; i++)
            {
                //创建子随机数种子
                RandomSeed r = seed.Add(bs[i].BuildingName.GetHash());
                //获得该类建筑生成总量
                int Count = GetCount(r.Add(1),new int2(length,width),bs[i].probability);//此处的add是为了确保每个步骤的种子不一样
                if (Count == 0) continue;//如果该建筑总数为零，忽略
                //逐个生成建筑
                RandomSeed r2 = r.Add(2);
                for (int j = 0; j < Count; j++)
                {
                    r2.Next();
                    //获得建筑创建类
                    IBuildingCreator bc;
                    try
                    {
                        bc = (IBuildingCreator)Activator.CreateInstance(Assembly.GetExecutingAssembly().GetType(bs[i].BuildingCreatorName));
                    }
                    catch
                    {
                        throw new ModAssetException("无法获取地形创建类", bs[i].GetAssetModName(), bs[i].GetAssetName());
                    }
                    int2 size = bc.GetNext(r2);
                    r2.Next();
                    int2 StartPosition = FindStartPosition(map, bs[i].dependences, size, r2);
                    if (StartPosition.x == -1) continue;//如果没找到位置就生成下一个
                    r2.Next();
                    bc.Creat(new BlockMap(map, StartPosition, size), size, r2);
                }

            }
        }
        /// <summary>
        /// 寻找建筑可以生成的位置，找失败
        /// </summary>
        public static int2 FindStartPosition(SceneMap map,TerrainDependences dependences,int2 size,RandomSeed seed)
        {
            if (map.IsOutOfRange(size))
                throw new ArgumentOutOfRangeException("size", "建筑大小超出了场景大小！");
            var random = seed.GetRandom();
            for(int t = 0; t < TryTimes; t++)
            {
                int2 startPosition = random.NextInt2(new int2(0, 0), map.Size - size);//生成位置
                int2 endPosition = startPosition + size;
                //检测该生成区域是否满足条件
                List<string> Terrains = new List<string>();//该区域内所有地形类型
                int3 position = new int3(0, 0, 0);
                for(position.x = startPosition.x;position.x < endPosition.x; position.x++)
                {
                    for(position.y = startPosition.y;position.y<endPosition.y;position.y++)
                    {

                        //如果当前位置有Object，代表该区域已被建筑使用，重新寻找
                        if(map.Objects.ContainsKey(position))
                        {
                            goto EndFind;
                        }
                        string TerrainName = map.TileTypes[position].GetAssetFullName();
                        if (!Terrains.Contains(TerrainName))
                        {
                            Terrains.Add(TerrainName);
                        }
                    }
                }
                if (dependences.IsMatch(Terrains))//如果该区域内的所有地形类型满足，返回该坐标
                {
                    return startPosition;
                }
                EndFind:
                continue;
            }
            return new int2(-1, -1);
        }
        private void CreateBiomes(SceneMap map, RandomSeed seed)
        {
            List<SceneBiome> bs = MatchBiomes(Terrains);
            for(int i = 0; i < bs.Count; i++)
            {
                IBiomeCreator bc;
                try
                {
                    bc = (IBiomeCreator)Activator.CreateInstance(Assembly.GetExecutingAssembly().GetType(bs[i].BiomeCreatorName));
                }
                catch
                {
                    throw new ModAssetException("无法获取群系创建类", bs[i].GetAssetModName(), bs[i].GetAssetName());
                }
                bc.Create(map, seed.Add(bs[i].BiomeName.GetHash()));
            }
        }
        /// <summary>
        /// 获得一个区域内可能生成的目标建筑的数量
        /// </summary>
        /// <param name="size">区域大小</param>
        /// <param name="probability">建筑生成概率（每100*100）</param>
        /// <returns>数量</returns>
        public static int GetCount(RandomSeed seed,int2 size,float probability)
        {
            int RandomCount = size.x * size.y / 10000;//计算该区域内100*100区块的数量
            int result = 0;
            Unity.Mathematics.Random r = seed.GetRandom();
            for(int i = 0;i< RandomCount; i++)
            {
                //计算该100*100是否会生成该建筑，如果会，建筑总数+1
                if(r.NextFloat(0f,1f) < probability)
                {
                    result += 1;
                }
            }
            return result;
        }
        /// <summary>
        /// 匹配出适合该地形列表的建筑
        /// </summary>
        public static List<SceneBuilding> MatchBuildings(List<string> Terrains)
        {
            List<SceneBuilding> Buildings = SaveManager.Active.GetAll<SceneBuilding>();
            List<SceneBuilding> result = new List<SceneBuilding>();
            for(int i = 0; i < Buildings.Count; i++)
            {
                //挨个筛选
                if (Buildings[i].dependences.IsMatch(Terrains))
                {
                    result.Add(Buildings[i]);
                }
            }
            return result;
        }
        /// <summary>
        /// 匹配出适合该地形列表的群系
        /// </summary>
        public static List<SceneBiome> MatchBiomes(List<string> Terrains)
        {
            List<SceneBiome> Biomes = SaveManager.Active.GetAll<SceneBiome>();
            List<SceneBiome> result = new List<SceneBiome>();
            for (int i = 0; i < Biomes.Count; i++)
            {
                //挨个筛选
                if (Biomes[i].dependences.IsMatch(Terrains))
                {
                    result.Add(Biomes[i]);
                }
            }
            return result;
        }

    }
}
