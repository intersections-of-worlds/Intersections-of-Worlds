using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using System.Collections.Generic;

namespace GameCore
{
    public interface ISceneMap
    {
        int2 Size { get; }
        List<(int3, Entity)> Entities { get; }
        Dictionary<int3, Entity> Objects { get; }
        Dictionary<int3, Entity> Tiles { get; }
        Dictionary<int3, AssetRef> TileTypes { get; }
        /// <summary>
        /// 检测一个坐标是否出界
        /// </summary>
        bool IsOutOfRange(int2 position);
        /// <summary>
        /// 检测一个坐标是否出界
        /// </summary>
        bool IsOutOfRange(int3 position);
        /// <summary>
        /// 在指定坐标放置实体（会动的！）
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="EntityType">实体类型</param>
        void SetEntity(int3 position, AssetRef EntityType);
        /// <summary>
        /// 在指定坐标放置物体（不会动的！），如果该坐标有物体会直接覆盖
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="ObjectType">物体类型</param>
        void SetObject(int3 position, AssetRef ObjectType);
        /// <summary>
        /// 在指定坐标添加地面（如果该坐标已有地面会直接覆盖）
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="GroundType">地面的类型</param>
        void SetTile(int3 position, AssetRef TileType);
        /// <summary>
        /// 在指定坐标放置物体（不会动的！）
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="ObjectType">物体类型</param>
        /// <returns>是否添加成功</returns>
        bool TrySetObject(int3 position, AssetRef ObjectType);
        /// <summary>
        /// 在指定坐标添加地面，如果该坐标已有地面会返回false
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="TileType">地面类型</param>
        /// <returns>是否添加成功</returns>
        bool TrySetTile(int3 position, AssetRef TileType);
    }
}