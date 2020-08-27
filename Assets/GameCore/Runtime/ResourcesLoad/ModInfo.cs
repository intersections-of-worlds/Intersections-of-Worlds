using UnityEngine;
using System;

namespace GameCore
{
    /// <summary>
    /// Mod的信息
    /// </summary>
    [CreateAssetMenu(menuName ="Intersections of Worlds/ModInfo")]
    [Serializable]
    public class ModInfo : ScriptableObject
    {
        /// <summary>
        /// Mod显示在外的名称
        /// </summary>
        public string ModName;
        /// <summary>
        /// Mod的内部名称，代码中的命名空间（实际命名空间名应为Mod.XX）。Mod包的名字为 内部名 + 空格 + 版本 ，内部名中请不要有空格
        /// Mod内三个系统组的名字为内部名+SimulationSystemGroup/InitializationSystemGroup/PresentationSystemGroup
        /// </summary>
        public string InternalName;
        /// <summary>
        /// Mod的唯一id
        /// </summary>
        public string ModId = Guid.NewGuid().ToString();
        /// <summary>
        /// Mod版本
        /// </summary>
        public Version version;
        /// <summary>
        /// Mod作者
        /// </summary>
        public string author;
        /// <summary>
        /// Mod的依赖Mod列表
        /// </summary>
        public ModMatcher[] dependences;
        /// <summary>
        /// Mod描述
        /// </summary>
        public string description;
        /// <summary>
        /// Mod的包名
        /// </summary>
        public string PackageName { get { return InternalName + " " + version.ToString(); } } 
        /// <summary>
        /// 将ModInfo转换成方便序列化成Json的形式
        /// </summary>
        /// <returns></returns>
        public ModInfoJsonObject ToJsonObject(){
            return new ModInfoJsonObject{
                ModName = ModName,
                InternalName = InternalName,
                ModId = ModId,
                version = version,
                author = author,
                dependences = dependences,
                description = description
            };
        }
    }
    /// <summary>
    /// 将ModInfo序列化成Json的中间层
    /// </summary>
    public class ModInfoJsonObject {
         /// <summary>
        /// Mod显示在外的名称
        /// </summary>
        public string ModName;
        /// <summary>
        /// Mod的内部名称，代码中的命名空间。Mod包的名字为 内部名 + 空格 + 版本 ，内部名中请不要有空格
        /// Mod内三个系统组的名字为内部名+SimulationSystemGroup/InitializationSystemGroup/PresentationSystemGroup
        /// </summary>
        public string InternalName;
        /// <summary>
        /// Mod的唯一id
        /// </summary>
        public string ModId;
        /// <summary>
        /// Mod版本
        /// </summary>
        public Version version;
        /// <summary>
        /// Mod作者
        /// </summary>
        public string author;
        /// <summary>
        /// Mod的依赖Mod列表
        /// </summary>
        public ModMatcher[] dependences;
        /// <summary>
        /// Mod描述
        /// </summary>
        public string description;
        /// <summary>
        /// 将中间层对象转换成真正的ModInfo对象
        /// </summary>
        /// <returns></returns>
        public ModInfo ToModInfo(){
            var result = ScriptableObject.CreateInstance<ModInfo>();
            result.ModName = ModName;
            result.InternalName = InternalName;
            result.ModId = ModId;
            result.version = version;
            result.author = author;
            result.dependences = dependences;
            result.description = description;
            return result;

        }

    }
}
