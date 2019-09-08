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
        public SaveArguments args;
        public SaveInfo(string name,RandomSeed saveSeed)
        {
            Name = name;
            Mods = new ModMatcherList();
            SaveSeed = saveSeed;
            args = new SaveArguments();
        }
    }
    [Serializable]
    public class SaveArguments
    {
        public int DefaultSceneNum = 3;
    }

}
