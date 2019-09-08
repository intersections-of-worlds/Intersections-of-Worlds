using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameCore
{
    public static class Paths
    {
        public readonly static string SavesPath = Application.persistentDataPath + "/Saves";
        public readonly static string ModsPath = Application.persistentDataPath + "/Mods";
    }
}
