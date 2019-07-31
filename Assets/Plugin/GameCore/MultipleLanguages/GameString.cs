using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
namespace GameCore
{
    public enum Language : int
    {
        None = -1,
        Englsih = 0,
        Chinese = 1
    }
    [Serializable]
    public struct GameString
    {
        /// <summary>
        /// 映射表的唯一标识
        /// </summary>
        private int dicid;
        /// <summary>
        /// string在映射表中的id
        /// </summary>
        private int id;
        /// <summary>
        /// 用一个string给自己设置值
        /// </summary>
        /// <param name="s">要设置的string</param>
        /// <param name="language">要设置的string的语言</param>
        public GameString(string s,Language lan = Language.None,int _dicid = 0)
        {
            //检测有没有初始化
            if (sd == null)
                throw new DidntInitException();
            if (_dicid == 0)
            {
                //如果不知道到映射表的id，就搜遍所有映射表
                foreach (var pair in sd)
                {
                    //dic为一个映射表
                    var dic = pair.Value;
                    //如果没有指定s的语言，就搜索所有语言来找到s对应的id
                    for (int i = 0; i < sd.Count; i++)
                    {
                        if (lan == Language.None)
                        {
                            foreach (var value in dic[i].Values)
                            {
                                if (value == s)
                                {
                                    dicid = pair.Key;
                                    id = i;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            //否则只搜索指定语言
                            if (dic[i][lan] == s)
                            {
                                dicid = pair.Key;
                                id = i;
                                return;
                            }
                        }
                    }
                }



            }
            else
            {
                //否则指搜对应映射表，当然，先检测该映射表是否存在
                if (!sd.ContainsKey(_dicid))
                    throw new ArgumentException("该映射表未被添加到主映射表中！！！你忘记Add了！", "_dicid");
                var dic = sd[_dicid];
                //如果没有指定s的语言，就搜索所有语言来找到s对应的id
                for (int i = 0; i < sd.Count; i++)
                {
                    if (lan == Language.None)
                    {
                        foreach (var value in dic[i].Values)
                        {
                            if (value == s)
                            {
                                dicid = _dicid;
                                id = i;
                                return;
                            }
                        }
                    }
                    else
                    {
                        //否则只搜索指定语言
                        if (dic[i][lan] == s)
                        {
                            dicid = _dicid;
                            id = i;
                            return;
                        }
                    }
                }

            }
            //如果没有搜到，抛出错误
            throw new ArgumentException("s不在映射表之中", "s");
        }
        /// <summary>
        /// 将自己转化成一个string
        /// </summary>
        /// <returns>转化成的string</returns>
        public string Get()
        {
            //检测有没有初始化
            if(sd == null)
            {
                throw new DidntInitException();
            }
            //检测自己所在的映射表是否已经被添加到主映射表中
            if (!sd.ContainsKey(dicid))
                throw new ArgumentException("该映射表未被添加到主映射表中！！！你忘记Add了！", "dicid");
            var q = sd[dicid][id];
            //如果有对应语言的string就直接返回
            if (q.ContainsKey(language))
                return q[language];
            else
            {
                //如果没有就找列表中第一个语言的string返回
                return q.Values.First();
            }
        }
        /// <summary>
        /// 当前语言环境
        /// </summary>
        private static Language language;
        /// <summary>
        /// 所有语言的string映射表
        /// </summary>
        private static Dictionary<int,List<Dictionary<Language,string>>> sd;
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init(Language lan)
        {
            //设置默认语言
            language = lan;
            sd = new Dictionary<int, List<Dictionary<Language, string>>>();
        }
        /// <summary>
        /// 添加string映射表
        /// </summary>
        public static void AddDic(params LanguageDictionary[] dics)
        {
            //检测有没有初始化
            if (sd == null)
            {
                throw new DidntInitException();
            }
            //挨个将string映射表添加进主映射表
            foreach (var dic in dics)
            {
                sd.Add(dic.id, dic.stringdic);
            }
        }
        /// <summary>
        /// 删除string映射表
        /// </summary>
        public static void RemoveDic(int dicid)
        {
            sd.Remove(dicid);
        }

    }
    public class DidntInitException :Exception
    {
        public DidntInitException() : base("本类未初始化")
        {

        }
    }
}
