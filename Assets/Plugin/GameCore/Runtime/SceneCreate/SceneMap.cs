using System;
using Unity.Mathematics;
using Unity.Collections;

namespace GameCore
{
    public struct SceneMap : IDisposable
    {

        public SceneMap(int length, int width)
        {
            Size = new int2(length, width);
            Tiles = new NativeHashMap<int3, AssetRef>(10, Allocator.TempJob);
            Objects = new NativeHashMap<int3, AssetRef>(10, Allocator.TempJob);
            Entities = new NativeList<(int3, AssetRef)>(Allocator.TempJob);
        }
        public int2 Size;
        public NativeHashMap<int3, AssetRef> Tiles { get; private set; }
        public NativeHashMap<int3, AssetRef> Objects { get; private set; }
        public NativeList<ValueTuple<int3, AssetRef>> Entities { get; private set; }
        /// <summary>
        /// 在指定坐标添加地面（如果该坐标已有地面会直接覆盖）
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="GroundType">地面的类型</param>
        public void SetTile(int3 position, AssetRef TileType)
        {
            if (IsOutOfRange(position))
                throw new ArgumentOutOfRangeException("position");
            if (Tiles.TryAdd(position, TileType))
            {
                //如果不存在就直接添加
            }
            else
            {
                //存在就覆盖掉原来的
                Tiles.Remove(position);
                SetTile(position, TileType);
            }
        }
        /// <summary>
        /// 在指定坐标添加地面，如果该坐标已有地面会返回false
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="TileType">地面类型</param>
        /// <returns>是否添加成功</returns>
        public bool TrySetTile(int3 position, AssetRef TileType)
        {
            if (IsOutOfRange(position))
                throw new ArgumentOutOfRangeException("position");
            return Tiles.TryAdd(position, TileType);
        }
        /// <summary>
        /// 在制定坐标放置物体（不会动的！），如果该坐标有物体会直接覆盖
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="ObjectType">物体类型</param>
        public void SetObject(int3 position, AssetRef ObjectType)
        {
            if (IsOutOfRange(position))
                throw new ArgumentOutOfRangeException("position");
            if (Objects.TryAdd(position, ObjectType))
            {
                //如果不存在就直接添加
            }
            else
            {
                //存在就覆盖掉原来的
                Objects.Remove(position);
                SetObject(position, ObjectType);
            }
        }
        /// <summary>
        /// 在制定坐标放置物体（不会动的！）
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="ObjectType">物体类型</param>
        /// <returns>是否添加成功</returns>
        public bool TrySetObject(int3 position, AssetRef ObjectType)
        {
            if (IsOutOfRange(position))
                throw new ArgumentOutOfRangeException("position");
            return Objects.TryAdd(position, ObjectType);
        }
        /// <summary>
        /// 在指定坐标放置实体（会动的！）
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="EntityType">实体类型</param>
        public void SetEntity(int3 position, AssetRef EntityType)
        {
            if (IsOutOfRange(position))
                throw new ArgumentOutOfRangeException("position");
            //不处理实体的碰撞问题
            Entities.Add((position, EntityType));
        }
        /// <summary>
        /// 检测一个坐标是否出界
        /// </summary>
        public bool IsOutOfRange(int2 position)
        {
            return Size.x <= position.x || Size.y <= position.y || position.x >= 0 || position.y >= 0;
        }
        public bool IsOutOfRange(int3 position)
        {
            return IsOutOfRange(ToInt2(ref position));
        }
        public void Dispose()
        {
            Tiles.Dispose();
            Objects.Dispose();
            Entities.Dispose();
        }
        /// <summary>
        /// 将一个map里的东西添加到自己的map里（目标map不能出界！）
        /// </summary>
        /// <param name="position">要将map添加到的位置（</param>
        public void Add(ref SceneMap map, int3 position)
        {
            var p2 = ToInt2(ref position) + map.Size;
            if (IsOutOfRange(p2))
                //对顶点出界就抛出错误
                throw new ArgumentOutOfRangeException();
            var keys = map.Tiles.GetKeyArray(Allocator.Temp);
            var values = map.Tiles.GetValueArray(Allocator.Temp);
            for (int i = 0; i < keys.Length; i++)
                //加上偏移
                SetTile(keys[i] + position, values[i]);
            keys.Dispose();//记得释放
            values.Dispose();
            keys = map.Objects.GetKeyArray(Allocator.Temp);
            values = map.Objects.GetValueArray(Allocator.Temp);
            for (int i = 0; i < keys.Length; i++)
                SetObject(keys[i] + position, values[i]);
            keys.Dispose();
            values.Dispose();
            for (int i = 0; i < map.Entities.Length; i++)
                SetEntity(map.Entities[i].Item1 + position, map.Entities[i].Item2);
        }
        public int2 ToInt2(ref int3 i)
        {
            return new int2(i.x, i.y);
        }
    }

}