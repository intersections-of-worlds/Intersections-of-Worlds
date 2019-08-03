using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

namespace GameCore
{
    public static class IJobBuildingCreatorExtention
    {
        /// <summary>
        /// 创建建筑（本函数不负责调用JobHandle.ScheduleBatchedJobs）
        /// </summary>
        public static Result Creat<T>(this T obj,int2 size,RandomSeed seed) where T : struct,IJobBuildingCreator
        {
            obj.size = size;
            obj.seed = seed;
            obj.Map = new NativeArray<SceneMap>(1, Allocator.TempJob);
            return new Result(obj.Schedule(), obj.Map);
        }
        public struct Result
        {
            private JobHandle Job;
            private NativeArray<SceneMap> result;

            public Result(JobHandle job, NativeArray<SceneMap> result)
            {
                Job = job;
                this.result = result;
            }
            public SceneMap GetResult()
            {
                Job.Complete();
                return result[0];
            }
            public bool IsComplete { get { return Job.IsCompleted; } }
        }

    }

}