using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using System.IO;
using System;
using UnityEditor.Build.Pipeline.Interfaces;
using GameCore;
using Newtonsoft.Json;
namespace GameCoreEditor
{
    public class ModBundleBuilder
    {
        static string OutPutFolder = Application.streamingAssetsPath + "/Mods";
        [MenuItem("Intersections of Worlds/Mod/Build（Only Editor）")]
        public static void Build()
        {
            Clear();
            var Mods = ModsEditor.GetMods();
            int length = Mods.Length;
            for (int i = 0; i < length; i++)
            {
                var ModInfo = ModsEditor.GetModInfo(Mods[i]);
                var ModOutPutFolder = OutPutFolder + "/" + ModInfo.PackageName;
                Directory.CreateDirectory(ModOutPutFolder);

                //生成ModInfo文件
                using (StreamWriter sw = File.CreateText(ModOutPutFolder + "/" + ModInfo.PackageName + ".json"))
                {
                    sw.Write(JsonConvert.SerializeObject(ModInfo));
                }

                //Windows64版本构建
                //构建参数
                var param = new BundleBuildParameters(BuildTarget.StandaloneWindows64, BuildTargetGroup.Standalone, ModOutPutFolder);
                param.BundleCompression = BuildCompression.LZ4;
                //填入资源
                var content = new BundleBuildContent(new AssetBundleBuild[] { GetAllAssets(ModInfo, "winmod") });
                IBundleBuildResults results;
                //构建包
                ReturnCode code = ContentPipeline.BuildAssetBundles(param, content, out results);
                if (code != ReturnCode.Success)
                {
                    if (code == ReturnCode.Canceled)
                        return;//如果取消，直接返回
                    Debug.LogError("构建失败！错误原因：" + code.ToString());
                }


            }
        }
        [MenuItem("Intersections of Worlds/Mod/Build（All Platform）")]
        public static void BuildAll()
        {
            Clear();
            var Mods = ModsEditor.GetMods();
            int length = Mods.Length;
            for (int i = 0; i < length; i++)
            {
                var ModInfo = ModsEditor.GetModInfo(Mods[i]);
                var ModOutPutFolder = OutPutFolder + "/" + ModInfo.PackageName;
                //Windows64版本构建
                //构建参数
                var param = new BundleBuildParameters(BuildTarget.StandaloneWindows64, BuildTargetGroup.Standalone, ModOutPutFolder);
                param.BundleCompression = BuildCompression.LZ4;
                //填入资源
                var content = new BundleBuildContent(new AssetBundleBuild[] { GetAllAssets(ModInfo, "winmod") });
                IBundleBuildResults results;
                //构建包
                ReturnCode code = ContentPipeline.BuildAssetBundles(param, content, out results);
                if (code != ReturnCode.Success)
                {
                    if (code == ReturnCode.Canceled)
                        return;//如果取消，直接返回
                    Debug.LogError("构建失败！错误原因：" + code.ToString());
                }

                //OSX版本构建
                //构建参数
                param.Target = BuildTarget.StandaloneOSX;
                //填入资源
                content = new BundleBuildContent(new AssetBundleBuild[] { GetAllAssets(ModInfo, "osxmod") });
                results = null;
                //构建包
                code = ContentPipeline.BuildAssetBundles(param, content, out results);
                if (code != ReturnCode.Success)
                {
                    if (code == ReturnCode.Canceled)
                        return;//如果取消，直接返回
                    Debug.LogError("构建失败！错误原因：" + code.ToString());
                }

                //Linux版本构建
                //构建参数
                param.Target = BuildTarget.StandaloneLinux64;
                //填入资源
                content = new BundleBuildContent(new AssetBundleBuild[] { GetAllAssets(ModInfo, "linuxmod") });
                results = null;
                //构建包
                code = ContentPipeline.BuildAssetBundles(param, content, out results);
                if (code != ReturnCode.Success)
                {
                    if (code == ReturnCode.Canceled)
                        return;//如果取消，直接返回
                    Debug.LogError("构建失败！错误原因：" + code.ToString());
                }

                //Android版本构建
                //构建参数
                param.Target = BuildTarget.Android;
                param.Group = BuildTargetGroup.Android;
                //填入资源
                content = new BundleBuildContent(new AssetBundleBuild[] { GetAllAssets(ModInfo, "androidmod") });
                results = null;
                //构建包
                code = ContentPipeline.BuildAssetBundles(param, content, out results);
                if (code != ReturnCode.Success)
                {
                    if (code == ReturnCode.Canceled)
                        return;//如果取消，直接返回
                    Debug.LogError("构建失败！错误原因：" + code.ToString());
                }

                //ios版本构建
                //构建参数
                param.Target = BuildTarget.iOS;
                param.Group = BuildTargetGroup.iOS;
                //填入资源
                content = new BundleBuildContent(new AssetBundleBuild[] { GetAllAssets(ModInfo, "iosmod") });
                results = null;
                //构建包
                code = ContentPipeline.BuildAssetBundles(param, content, out results);
                if (code != ReturnCode.Success)
                {
                    if (code == ReturnCode.Canceled)
                        return;//如果取消，直接返回
                    Debug.LogError("构建失败！错误原因：" + code.ToString());
                }

                //生成ModInfo文件
                ModInfo = ModsEditor.GetModInfo(Mods[i]);//由于构建后资源会被释放，这里需要重载
                using (FileStream fs = File.Create(ModOutPutFolder + "/" + ModInfo.PackageName + ".json"))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(JsonConvert.SerializeObject(ModInfo));
                    sw.Dispose();
                }
            }
        }
        public static void Clear()
        {
            if (Directory.Exists(OutPutFolder))
            {
                Directory.Delete(OutPutFolder, true);
            }
        }
        public static AssetBundleBuild GetAllAssets(ModInfo info, string platform)
        {
            AssetIndexer ai = ModsEditor.GetAssetIndexer(info.InternalName);
            AssetBundleBuild abb = new AssetBundleBuild();
            abb.assetBundleName = info.PackageName + "." + platform;
            abb.addressableNames = new string[ai.InfoList.Count + 3];
            abb.assetNames = new string[ai.InfoList.Count + 3];
            int i = 0;
            for (i = 0; i < ai.InfoList.Count; i++)
            {
                abb.addressableNames[i] = ai.InfoList[i].FullName;
                abb.assetNames[i] = AssetDatabase.GUIDToAssetPath(ai[i].AssetId.ToString());
            }
            //添加包信息内容
            abb.addressableNames[i] = "ModInfo";
            abb.assetNames[i++] = ModsEditor.GetModInfoPath(info.InternalName);

            abb.addressableNames[i] = "AssetIndexer";
            abb.assetNames[i++] = ModsEditor.GetAssetIndexerPath(info.InternalName);

            abb.addressableNames[i] = "LanguageDic";
            abb.assetNames[i++] = ModsEditor.GetLanguageDicPath(info.InternalName);
            return abb;
        }
    }
}
