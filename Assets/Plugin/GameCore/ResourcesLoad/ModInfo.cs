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
        /// Mod的内部名称，代码中的命名空间。Mod包的名字为 内部名 + 空格 + 版本 ，内部名中请不要有空格
        /// Mod内三个系统组的名字为内部名+SimulationSystemGroup/InitializationSystemGroup/PresentationSystemGroup
        /// </summary>
        public string InternalName;
        /// <summary>
        /// Mod的唯一id
        /// </summary>
        public int ModId = Guid.NewGuid().GetHashCode();
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
        /// Mod包的类型
        /// </summary>
        public BundleType bundleType;
        /// <summary>
        /// Mod的包名
        /// </summary>
        public string PackageName { get { return InternalName + " " + version.ToString(); } } 
    }
}
