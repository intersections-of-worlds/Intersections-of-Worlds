using Unity.Mathematics;

namespace GameCore
{
    /// <summary>
    /// 创建建筑的类的接口
    /// </summary>
    public interface IBuildingCreator
    {
        /// <summary>
        /// 获得可能生成的下一个建筑的占地大小（长宽）
        /// </summary>
        /// <returns>大小</returns>
        int2 GetNext(RandomSeed seed);
        /// <summary>
        /// 创建一个指定大小的建筑
        /// </summary>
        SceneMap Creat(int2 size,RandomSeed seed);
    }

}