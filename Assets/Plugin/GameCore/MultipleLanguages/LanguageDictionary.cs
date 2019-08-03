using System.Collections.Generic;
using System;
using UnityEngine;
namespace GameCore
{
    [CreateAssetMenu(menuName = "Intersections of Worlds/LanguageDictionary")]
    [Serializable]
    public class LanguageDictionary : ScriptableObject
    {
        public int id = Guid.NewGuid().GetHashCode();

        public List<Dictionary<Language, string>> stringdic;
    }
}
