using Unity.Entities;
using Unity.Collections;

namespace GameCore
{
    public abstract class DataStoreSystem : ComponentSystem
    {
        /// <summary>
        /// 已序列化待储存的数据
        /// </summary>
        protected NativeQueue<(Identification, NativeList<byte>)> SerializedData = new NativeQueue<(Identification, NativeList<byte>)>();
        public NativeQueue<(Identification, NativeList<byte>)>.ParallelWriter GetStorer()
        {
            return SerializedData.AsParallelWriter();
        }
    }

}