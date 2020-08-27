using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameCore
{
    public static class Paths
    {
        /// <summary>
        /// 存档路径
        /// </summary>
        public readonly static string SavesPath = Application.persistentDataPath + "/Saves";
        /// <summary>
        /// Mod路径
        /// </summary>
        public readonly static string ModsPath = Application.persistentDataPath + "/Mods";
    }
}
