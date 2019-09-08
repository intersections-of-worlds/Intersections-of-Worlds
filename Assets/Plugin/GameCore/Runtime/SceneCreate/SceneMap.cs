using System;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using System.Collections.Generic;

namespace GameCore
{
    public class SceneMap : ISceneMap
    {
        public SceneMap(SceneManager scene, int length, int width)
        {
            Scene = scene;
            Size = new int2(length, width);
            Tiles = new Dictionary<int3, Entity>();
            Objects = new Dictionary<int3, Entity>();
            Entities = new List<(int3, Entity)>();
            TileTypes = new Dictionary<int3, AssetRef>();
        }
        public int2 Size { get; private set; }

        public SceneManager Scene { get; private set; }
        public Dictionary<int3, Entity> Tiles { get; private set; }
        public Dictionary<int3, Entity> Objects { get; private set; }
        public List<ValueTuple<int3, Entity>> Entities { get; private set; }

        public Dictionary<int3, AssetRef> TileTypes { get; private set; }

        /// <summary>
        /// 在指定坐标添加地面（如果该坐标已有地面会直接覆盖）
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="GroundType">地面的类型</param>
        public virtual void SetTile(int3 position, AssetRef TileType)
        {
            if (IsOutOfRange(position))
                throw new ArgumentOutOfRangeException("position");
            Scene.CreateObject(TileType, (e, b) =>
             {
                 SetPosition(e, position);
                 if (!Tiles.ContainsKey(position))
                 {
                    //如果不存在就直接添加
                    Tiles.Add(position, e);
                     TileTypes.Add(position, TileType);
                 }
                 else
                 {
                    //存在就覆盖掉原来的并删除原来的
                    WorldObjectManager.Main.DeleteObject(Tiles[position]);
                     Tiles.Remove(position);
                     TileTypes.Remove(position);
                     Tiles.Add(position, e);
                     TileTypes.Add(position, TileType);
                 }
             });
        }
        /// <summary>
        /// 在指定坐标添加地面，如果该坐标已有地面会返回false
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="TileType">地面类型</param>
        /// <returns>是否添加成功</returns>
        public virtual bool TrySetTile(int3 position, AssetRef TileType)
        {
            if (IsOutOfRange(position))
                throw new ArgumentOutOfRangeException("position");
            if (!Tiles.ContainsKey(position))
                return false;
            Scene.CreateObject( TileType,(e, b) =>
             {
                SetPosition(e, position);
                Tiles.Add(position, e);
            });
            return true;
        }
        /// <summary>
        /// 在指定坐标放置物体（不会动的！），如果该坐标有物体会直接覆盖
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="ObjectType">物体类型</param>
        public virtual void SetObject(int3 position, AssetRef ObjectType)
        {
            if (IsOutOfRange(position))
                throw new ArgumentOutOfRangeException("position");
            Scene.CreateObject(ObjectType, (e, b) =>
           {
               SetPosition(e, position);
               if (!Objects.ContainsKey(position))
               {
                    //如果不存在就直接添加
                    Objects.Add(position, e);
               }
               else
               {
                    //存在就覆盖掉原来的
                    WorldObjectManager.Main.DeleteObject(Objects[position]);
                   Objects.Remove(position);
                   Objects.Add(position, e);
               }
           });
        }
        /// <summary>
        /// 在指定坐标放置物体（不会动的！）
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="ObjectType">物体类型</param>
        /// <returns>是否添加成功</returns>
        public virtual bool TrySetObject(int3 position, AssetRef ObjectType)
        {
            if (IsOutOfRange(position))
                throw new ArgumentOutOfRangeException("position");
            if (!Objects.ContainsKey(position))
                return false;
            Scene.CreateObject(ObjectType, (e, b) =>
           {
               SetPosition(e, position);
               Objects.Add(position, e);
           });
            return true;
        }
        /// <summary>
        /// 在指定坐标放置实体（会动的！）
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="EntityType">实体类型</param>
        public virtual void SetEntity(int3 position, AssetRef EntityType)
        {
            if (IsOutOfRange(position))
                throw new ArgumentOutOfRangeException("position");
            Scene.CreateObject(EntityType, (e, b) =>
            {
                SetPosition(e, position);
                //不处理实体的碰撞问题
                Entities.Add((position, e));
            });
        }
        /// <summary>
        /// 检测一个坐标是否出界
        /// </summary>
        public virtual bool IsOutOfRange(int2 position)
        {
            return Size.x <= position.x || Size.y <= position.y || position.x < 0 || position.y < 0;
        }
        /// <summary>
        /// 检测一个坐标是否出界
        /// </summary>
        public virtual bool IsOutOfRange(int3 position)
        {
            return IsOutOfRange(ToInt2(position));
        }
        public static int2 ToInt2(int3 i)
        {
            return new int2(i.x, i.y);
        }
        /// <summary>
        /// 设置实体位置
        /// </summary>
        protected virtual void SetPosition(Entity e,int3 position)
        {
            World.Active.EntityManager.SetComponentData(e, new Translation { Value = position });
        }
    }
    /// <summary>
    /// 场景中一个相对区块的Map
    /// </summary>
    public class BlockMap : ISceneMap
    {
        private SceneMap map;

        public BlockMap(SceneMap Map,int2 startPosition,int2 size)
        {
            //如果出界了，抛出错误
            if (Map.IsOutOfRange(startPosition + size))
            {
                throw new System.ArgumentOutOfRangeException("size", "范围超出了场景的大小！");
            }
            map = Map;
            StartPosition = startPosition;
            Size = size;
        }

        public int2 StartPosition { get; private set; }
        public int2 Size { get; private set; }

        List<(int3, Entity)> ISceneMap.Entities => ((ISceneMap)map).Entities;

        Dictionary<int3, Entity> ISceneMap.Objects => ((ISceneMap)map).Objects;

        Dictionary<int3, Entity> ISceneMap.Tiles => ((ISceneMap)map).Tiles;

        Dictionary<int3, AssetRef> ISceneMap.TileTypes => ((ISceneMap)map).TileTypes;

        public bool IsOutOfRange(int2 position)
        {

            return position.x >= Size.x || position.y >= Size.y ||
                position.x < 0 || position.y < 0;
        }

        public bool IsOutOfRange(int3 position)
        {
            return IsOutOfRange(SceneMap.ToInt2(position));
        }

        public void SetEntity(int3 position, AssetRef EntityType)
        {
            if (IsOutOfRange(position))
                throw new System.ArgumentOutOfRangeException( "position", "坐标超出了范围！");
            int3 Position = new int3(position.x + StartPosition.x, position.y + StartPosition.y, position.z);
            map.SetEntity(Position, EntityType);
        }

        public void SetObject(int3 position, AssetRef ObjectType)
        {
            if (IsOutOfRange(position))
                throw new System.ArgumentOutOfRangeException("position", "坐标超出了范围！");
            int3 Position = new int3(position.x + StartPosition.x, position.y + StartPosition.y, position.z);
            map.SetObject(Position, ObjectType);
        }

        public void SetTile(int3 position, AssetRef TileType)
        {
            if (IsOutOfRange(position))
                throw new System.ArgumentOutOfRangeException("position", "坐标超出了范围！");
            int3 Position = new int3(position.x + StartPosition.x, position.y + StartPosition.y, position.z);
            map.SetTile(Position, TileType);
        }

        public bool TrySetObject(int3 position, AssetRef ObjectType)
        {
            if (IsOutOfRange(position))
                throw new System.ArgumentOutOfRangeException("position", "坐标超出了范围！");
            int3 Position = new int3(position.x + StartPosition.x, position.y + StartPosition.y, position.z);
            return map.TrySetObject(Position, ObjectType);
        }

        public bool TrySetTile(int3 position, AssetRef TileType)
        {
            if (IsOutOfRange(position))
                throw new System.ArgumentOutOfRangeException("position", "坐标超出了范围！");
            int3 Position = new int3(position.x + StartPosition.x, position.y + StartPosition.y, position.z);
            return map.TrySetTile(Position, TileType);
        }
    }
}