using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameCore.Serialization
{
    /// <summary>
    /// 组件数据过滤器
    /// </summary>
    struct DataFilter<T> : IJob where T : struct
    {
        /// <summary>
        /// 要过滤的实体的标识
        /// </summary>
        public NativeArray<Identification> identifications;
        /// <summary>
        /// 要过滤的实体的组件
        /// </summary>
        public NativeArray<T> datas;
        /// <summary>
        /// 要过滤的场景id
        /// </summary>
        public NativeArray<int> Scenes;
        /// <summary>
        /// 过滤出的结果，索引与Scenes一一对应
        /// </summary>
        public NativeArray<NativeList<(Identification, T)>> eachscenedata;
        public void Execute()
        {
            for (int i = 0; i < identifications.Length; i++)
            {
                for (int j = 0; j < Scenes.Length; j++)
                {
                    //如果该组件所属实体是要存的某个场景的实体之一，将其添加到对应场景的组件列表中
                    if (identifications[i].SceneId == Scenes[j])
                    {
                        eachscenedata[j].Add((identifications[i], datas[i]));
                    }
                }
            }
            //筛选完后把筛选用数组释放掉
            identifications.Dispose();
            datas.Dispose();
            Scenes.Dispose();
            eachscenedata.Dispose();
        }
    }

}