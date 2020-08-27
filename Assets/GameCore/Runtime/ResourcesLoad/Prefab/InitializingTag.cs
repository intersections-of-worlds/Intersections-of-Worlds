using Unity.Entities;

namespace GameCore{
    /// <summary>
    /// 标签组件，用于标识该Entity是否正在初始化
    /// </summary>
    public struct InitializingTag : IComponentData
    {

    }
}