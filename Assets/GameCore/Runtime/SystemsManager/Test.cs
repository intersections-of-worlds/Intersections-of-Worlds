using Unity.Entities;

namespace GameCore{
    //关于系统加载的各种复杂情况的测试
    //没有定义世界与目标组的测试
    public class CommonSystem : SystemBase{
        protected override void OnUpdate(){

        }
    }
    //定义了世界没有定义目标组
    [UpdateInWorld(WorldTypes.ServerWorld)]
    public class ServerCommonSystem : SystemBase{
        protected override void OnUpdate(){

        }
    }
    /// <summary>
    /// 没有目标组但加入两个世界
    /// </summary>
    [UpdateInWorld(WorldTypes.ClientWorld | WorldTypes.ServerWorld)]
    public class BothCommonSystem : SystemBase{
        protected override void OnUpdate(){

        }
    }
    /// <summary>
    /// 没有目标组但加入默认世界
    /// </summary>
    [UpdateInWorld(WorldTypes.ExplicitDefaultWorld)]
    public class DefaultWorldCommonSystem : SystemBase{
        protected override void OnUpdate(){

        }
    }
    /// <summary>
    /// 加入ServerWorld中的Presentation组
    /// </summary>
    [UpdateInWorld(WorldTypes.ServerWorld)]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class ServerPresentationCommonSystem : SystemBase{
        protected override void OnUpdate(){

        }
    }
    /// <summary>
    /// 加入Server的自定义系统组
    /// </summary>
    
    [UpdateInGroup(typeof(ServerCommonSystemGroup))]
    public class ServerMyGorupSystem : SystemBase{
        protected override void OnUpdate(){

        }
    }
    
    [UpdateInWorld(WorldTypes.ServerWorld)]
    public class ServerCommonSystemGroup : ComponentSystemGroup{
    }
}