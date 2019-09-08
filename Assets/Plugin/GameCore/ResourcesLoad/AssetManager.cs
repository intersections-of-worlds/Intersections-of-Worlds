using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameCore
{
    /// <summary>
    /// Mod资源管理器
    /// </summary>
    public class AssetManager 
    {
        /// <summary>
        /// 资源所属的Mod
        /// </summary>
        public ModBundle Mod;
        /// <summary>
        /// 资源索引器
        /// </summary>
        public AssetIndexer Indexer;
        /// <summary>
        /// 资源储存在的包
        /// </summary>
        public AssetBundle AB;
        public AssetManager(ModBundle mod)
        {
            Mod = mod;
            AB = Mod.ab;
            Indexer = AB.LoadAsset<AssetIndexer>("AssetIndexer");
        }
        /// <summary>
        /// 从Mod中加载资源
        /// </summary>
        /// <typeparam name="T">资源的类型</typeparam>
        public virtual T Get<T>(string AssetFullName) where T : UnityEngine.Object
        {
            return AB.LoadAsset<T>(AssetFullName);
        }
        /// <summary>
        /// 从Mod中加载资源
        /// </summary>
        public virtual T Get<T>(int AssetId) where T : UnityEngine.Object
        {
            int index = Indexer.TryGet(AssetId);
            if (index == -1)
                return null;
            else
                return AB.LoadAsset<T>(Indexer[index].FullName);
        }
        /// <summary>
        /// 获得Mod中资源的引用
        /// </summary>
        public virtual AssetRef GetRef(string AssetFullName)
        {
            return Indexer[AssetFullName].GetRef();
        }
        /// <summary>
        /// 加载该Mod内所有该类型的资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        public virtual T[] GetAll<T>() where T : UnityEngine.Object
        {
            return AB.LoadAllAssets<T>();
        }
        /// <summary>
        /// 获得Mod中所有该类型资源的引用
        /// </summary>
        public virtual List<AssetRef> GetAllRef<T>() where T : UnityEngine.Object
        {
            List<AssetRef> result = new List<AssetRef>();
            for (int i = 0; i < Indexer.Count; i++)
            {
                if (Indexer[i].Is<T>())
                {
                    result.Add(Indexer[i].GetRef());
                }
            }
            return result;
        }
        /// <summary>
        /// 加载该Mod内所有匹配这些Tag的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="All">资源必须包含的Tag</param>
        /// <param name="None">资源不应包含的Tag</param>
        public virtual List<T> GetAllByTags<T>(string[] All, string[] None) where T : UnityEngine.Object
        {
            List<T> result = new List<T>();
            for (int i = 0; i < Indexer.Count; i++)
            {
                if (Indexer[i].IsMatchTag(All, None) && Indexer[i].Is(typeof(T)))
                    result.Add(AB.LoadAsset<T>(Indexer[i].FullName));
            }
            return result;
        }
        /// <summary>
        /// 获得该Mod内所有匹配这些Tag的资源的引用
        /// </summary>
        public virtual List<AssetRef> GetAllRefByTags<T>(string[] All, string[] None) where T : UnityEngine.Object
        {
            List<AssetRef> result = new List<AssetRef>();
            for (int i = 0; i < Indexer.Count; i++)
            {
                if (Indexer[i].IsMatchTag(All, None) && Indexer[i].Is(typeof(T)))
                    result.Add(Indexer[i].GetRef());
            }
            return result;
        }
        /// <summary>
        /// 尝试通过资源的id获取资源
        /// </summary>
        public virtual string TryGetAssetFullName(int AssetId)
        {
            return Indexer.TryGetAssetFullName(AssetId);
        }
        public virtual AssetInfo GetAssetInfo(string AssetFullName)
        {
            return Indexer[AssetFullName];
        }
    }
}
