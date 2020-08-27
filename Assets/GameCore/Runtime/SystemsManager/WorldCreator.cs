using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System.Reflection;
using ILRuntime.Runtime;
using System;
using System.Threading.Tasks;

namespace GameCore
{
    /// <summary>
    /// 自定义World生成
    /// </summary>
    public class WorldCreator : ICustomBootstrap
    {
        /// <summary>
        /// 原生Mod系统分类，非Mod系统统一置于一个NatvieModSystems中
        /// </summary>
        public class NativeModSystems
        {
            /// <summary>
            /// 命名空间名，基本系统为GameCore
            /// </summary>
            public string NamespaceName;
            /// <summary>
            /// 原本加入默认世界的系统，将同时加入ClientWorld和ServerWorld
            /// </summary>
            public List<Type> DefaultWorldSystem = new List<Type>();
            /// <summary>
            /// 加入服务端的系统
            /// </summary>
            public List<Type> ServerSystems = new List<Type>();
            /// <summary>
            /// 加入客户端的系统
            /// </summary>
            public List<Type> ClientSystems = new List<Type>();
            /// <summary>
            /// 原本加入默认世界且系统组为PresentationGroup的系统
            /// 方便在ServerWorld中禁止RenderSystem等更新而存在
            /// </summary>
            public List<Type> DefaultPresentationSystems = new List<Type>();
            public NativeModSystems(string namespaceName){
                NamespaceName = namespaceName;
            }
        }
        
        /// <summary>
        /// 明确声明要加入DefaultWorld的原生系统，如果没有明确声明将会同时加入ClientWorld和ServerWorld
        /// 如果明确声明，即使Mod没有被加载该系统也会被加载，因为默认世界创建顺序在存档启动（即服务端和客户端世界创建）之前
        /// </summary>
        public List<Type> ExplicitDefaultWorldSystems{get;private set;}
        /// <summary>
        /// 分类过的Mod以及属于它的System
        /// </summary>
        /// <value></value>
        public Dictionary<string,NativeModSystems> ModSystems{get;private set;}
        /// <summary>
        /// 被unity调用的初始化函数
        /// </summary>
        public bool Initialize(string DefaultWorldName)
        {
            //创建默认世界
            World defaultWorld = new World(DefaultWorldName);
            //将World里得默认世界设置为defaultWorld
            World.DefaultGameObjectInjectionWorld = defaultWorld;
            //获取所有原生的系统
            var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
            //对系统进行分类
            SortSystems(systems);
            //添加系统
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(defaultWorld,ExplicitDefaultWorldSystems);
            //将系统添加到Update列表中
            ScriptBehaviourUpdateOrder.AddWorldToCurrentPlayerLoop(defaultWorld);
            CreateClientWorld("ClientWorld","GameCore");
            CreateServerWorld("ServerWorld","GameCore");
            return true;
        }
        /// <summary>
        /// 创建客户端世界
        /// </summary>
        public World CreateClientWorld(string ClientWorldName,params string[] EnableMods){
            //创建世界
            World ClientWorld = new World(ClientWorldName);
            EntityManager manager = ClientWorld.EntityManager;
            //添加世界信息组件
            Entity e = manager.CreateEntity(ComponentType.ReadWrite<WorldTypeInfo>());
            manager.SetComponentData(e,new WorldTypeInfo(){type = WorldTypes.ClientWorld});
            manager.SetName(e,"WorldTypeInfo");
            //轮询添加系统
            for(int i = 0; i < EnableMods.Length;i++){
                NativeModSystems mod = ModSystems[EnableMods[i]];
                DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(ClientWorld, mod.DefaultWorldSystem);
                DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(ClientWorld, mod.ClientSystems);
            }
            //添加到Update列表
            ScriptBehaviourUpdateOrder.AddWorldToCurrentPlayerLoop(ClientWorld);
            return ClientWorld;
        }
        /// <summary>
        /// 创建服务端世界
        /// </summary>
        public World CreateServerWorld(string ServerWorldName,params string[] EnableMods){
            //创建世界
            World ServerWorld = new World(ServerWorldName);
            EntityManager manager = ServerWorld.EntityManager;
            //添加世界信息组件
            Entity e = manager.CreateEntity(ComponentType.ReadWrite<WorldTypeInfo>());
            manager.SetComponentData(e,new WorldTypeInfo(){type = WorldTypes.ServerWorld});
            manager.SetName(e,"WorldTypeInfo");
            //为接下来将在ServerWorld中禁用的Presentation组的系统做缓存，节省查找开销
            List<Type>[] types = new List<Type>[EnableMods.Length];
            //轮询添加系统
            for(int i = 0; i < EnableMods.Length;i++){
                NativeModSystems mod = ModSystems[EnableMods[i]];
                DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(ServerWorld, mod.DefaultWorldSystem);
                DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(ServerWorld, mod.ServerSystems);
                //缓存
                types[i] = mod.DefaultPresentationSystems;
            }
            ScriptBehaviourUpdateOrder.AddWorldToCurrentPlayerLoop(ServerWorld);
            //获取ServerWorld中的PresentationSystemGroup并禁止其运行
            for(int i = 0;i<types.Length;i++){
                for(int j = 0;j<types[i].Count;j++){
                    ServerWorld.GetExistingSystem(types[i][j]).Enabled = false;
                }
            }
            return ServerWorld;
        }
        /// <summary>
        /// 对原生系统进行分类
        /// </summary>
        public void SortSystems(IReadOnlyList<Type> systems)
        {
            //初始化
            ExplicitDefaultWorldSystems = new List<Type>();
            ModSystems = new Dictionary<string,NativeModSystems>();
            //提前缓存基本系统所处的分类表，避免之后多次查询
            var GameCoreSystems = new NativeModSystems("GameCore");
            ModSystems.Add("GameCore",GameCoreSystems);
            int i = 0;
            for (i = 0; i < systems.Count; i++)
            {
                
                //获得系统所属命名空间名
                string[] SystemName = TypeManager.GetSystemName(systems[i]).Split('.');
                //Mod的命名空间名格式为Mod.XX，
                string FirstName = SystemName[0];
                //避免出现名为Mod.XX的类导致错误分类，如果类名为Mod.XX将视作基本系统
                if(FirstName == "Mod" && SystemName.Length > 2){
                    //分类Mod系统
                    string SecondName = SystemName[1];
                    NativeModSystems mod;
                    if(ModSystems.TryGetValue(SecondName, out mod)){
                        //如果找到了就直接分类Mod内部系统，没有找到新建一个再分类
                        SortModSystem(systems[i],mod);
                    }else{
                        mod = new NativeModSystems(SecondName);
                        SortModSystem(systems[i],mod);
                        ModSystems.Add(SecondName,mod);
                    }
                }else{
                    //分类基本系统
                    SortModSystem(systems[i],GameCoreSystems);
                }
            }
        }
        /// <summary>
        /// 对每个Mod的系统根据运行于的世界和时间分类
        /// </summary>
        private void SortModSystem(Type system,NativeModSystems mod){
            //获得系统运行于的世界
            var worldtype = GetSystemWorldType(system);
            if(worldtype == WorldTypes.DefaultWorld){
                //是默认状态则添加到DefaultSystem中，由于默认状态的int为0所以要直接用=比较
                mod.DefaultWorldSystem.Add(system);
                //如果所属系统组为Presentation组，加入列表，将在服务端禁用
                var attr = TypeManager.GetSystemAttributes(system,typeof(UpdateInGroupAttribute));
                if(attr.Length != 0 && (attr[0] as UpdateInGroupAttribute).GroupType.Equals(typeof(PresentationSystemGroup))){
                    mod.DefaultPresentationSystems.Add(system);
                }
                return;//且不需要继续比较
            }
            if((worldtype & WorldTypes.ClientWorld) == WorldTypes.ClientWorld){
                //添加到客户端世界
                mod.ClientSystems.Add(system);
            }
            if((worldtype & WorldTypes.ServerWorld) == WorldTypes.ServerWorld){
                //添加到服务端世界
                mod.ServerSystems.Add(system);

            }
            if((worldtype & WorldTypes.ExplicitDefaultWorld) == WorldTypes.ExplicitDefaultWorld){
                //添加到默认世界
                ExplicitDefaultWorldSystems.Add(system);
            }
        }
        /// <summary>
        /// 获取一个系统应该运行于的世界
        /// </summary>
        public WorldTypes GetSystemWorldType(Type system){
            //获得worldType的特性，获取到的是个数组，但由于限制最多只会有一个内容物
            var worldtypeattrs = TypeManager.GetSystemAttributes(system,typeof(UpdateInWorldAttribute));
            //如果目标身上没有该特性，就查他运行于的系统组运行于的世界
            if(worldtypeattrs.Length == 0){
                var updateInGroupAttributes = TypeManager.GetSystemAttributes(system,typeof(UpdateInGroupAttribute));
                //如果目标身上也没有UpdateInGroup，按运行于默认世界的SimulationGroup处理
                if(updateInGroupAttributes.Length == 0){
                    return WorldTypes.DefaultWorld;
                }else{
                    //通过递归寻找所处系统组的WorldType
                    return GetSystemWorldType((updateInGroupAttributes[0] as UpdateInGroupAttribute).GroupType);
                }
            }
            return (worldtypeattrs[0] as UpdateInWorldAttribute).Types;
        }
    }
    /// <summary>
    /// 用于声明系统所运行的世界，该特性不可重复，可继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false,Inherited = true)]
    public class UpdateInWorldAttribute : Attribute{
        public WorldTypes Types{get;private set;}
        public UpdateInWorldAttribute(WorldTypes types){
            Types = types;
        }
        public UpdateInWorldAttribute(params WorldTypes[] types){
            Types = 0;
            for (int i = 0; i < types.Length;i++){
                Types |= types[i];
            }
        }
    }
    /// <summary>
    /// 世界的类型，可叠加
    /// </summary>
    [FlagsAttribute]
    public enum WorldTypes : int{
        //未声明，原本默认运行于默认世界，会被其他情况覆盖
        DefaultWorld = 0,
        //声明了要运行于默认世界
        ExplicitDefaultWorld = 1,
        //运行于客户端世界
        ClientWorld = 2,
        //运行于服务端世界
        ServerWorld = 4
    }
    /// <summary>
    /// 一个World只有一个的单例组件，标识这个World的类型
    /// </summary>
    public struct WorldTypeInfo : IComponentData{
        public WorldTypes type;
    }
}