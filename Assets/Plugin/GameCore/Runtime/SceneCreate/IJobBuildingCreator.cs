using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

namespace GameCore
{
    /// <summary>
    /// 多线程创建建筑的类的接口
    /// </summary>
    public interface IJobBuildingCreator : IJob
    {
        /// <summary>
        /// 获得可能生成的下一个建筑的占地大小（长宽）
        /// </summary>
        /// <returns>大小(长宽，高度不管）</returns>
        int2 GetNext(RandomSeed seed);
        /// <summary>
        /// 下一个建筑的大小
        /// </summary>
        int2 size { get; set; }
        /// <summary>
        /// 下一个建筑创建使用的种子
        /// </summary>
        RandomSeed seed { get; set; }
        /// <summary>
        /// 下一个建筑使用的Map
        /// </summary>
        NativeArray<SceneMap> Map { get; set; }


    }

}