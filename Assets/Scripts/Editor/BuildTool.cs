using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class BuildTool : Editor
{
    [MenuItem("Tools/Build Windows Bundles")]
    static void BuildWindowsBundles()
    {
        BuildBundles(BuildTarget.StandaloneWindows);
    }
    [MenuItem("Tools/Build Android Bundles")]
    static void BuildAndroidBundles()
    {
        BuildBundles(BuildTarget.Android);
    }
    [MenuItem("Tools/Build IOS Bundles")]
    static void BuildIosBundles()
    {
        BuildBundles(BuildTarget.iOS);
    }

    static void BuildBundles(BuildTarget target)
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();

        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta"))
                continue;

            AssetBundleBuild assetBundle = new AssetBundleBuild();

            string assetPath = PathUtil.GetUnityPath(files[i]);
            assetPath = PathUtil.GetStandardPath(assetPath);
            Debug.Log(assetPath);
            assetBundle.assetNames = new string[] { assetPath };
            string assetname = files[i].Replace(PathUtil.BuildResourcesPath, "").ToLower();
            assetBundle.assetBundleName = assetname + ".ab";
            assetBundleBuilds.Add(assetBundle);
        }

        if(Directory.Exists(PathUtil.BuildOutPath))
            Directory.Delete(PathUtil.BuildOutPath, true);
        Directory.CreateDirectory(PathUtil.BuildOutPath);

        BuildPipeline.BuildAssetBundles(PathUtil.BuildOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);
    }
}
