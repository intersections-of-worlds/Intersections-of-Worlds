using System;
namespace GameCore
{
    /// <summary>
    /// 储存存档信息的类
    /// </summary>
    [Serializable]
    public class SaveInfo
    {
        /// <summary>
        /// 存档名
        /// </summary>
        public string Name;
        /// <summary>
        ///该存档拥有的Mod列表
        /// </summary>
        public ModMatcherList Mods;
        /// <summary>
        /// 当前存档的种子
        /// </summary>
        public RandomSeed SaveSeed;
        /// <summary>
        /// 存档的配置信息
        /// </summary>
        public SaveConfig config;
        public SaveInfo(string name,RandomSeed saveSeed)
        {
            Name = name;
            Mods = new ModMatcherList();
            SaveSeed = saveSeed;
            config = new SaveConfig();
        }
    }
    [Serializable]
    public class SaveConfig
    {
        public int DefaultSceneNum = 3;
    }

}
