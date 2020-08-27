using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

namespace GameCore
{
    /// <summary>
    /// 游戏对象的唯一Id，即使场景变更也不会变
    /// </summary>
    [GenerateAuthoringComponent]
    public struct ObjectId : IComponentData
    {
        //因防止超界使用long
        public long Id;
    }
}