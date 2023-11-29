using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

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

        List<string> bundleInfos = new List<string>();

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
            string bundlePath = assetPath.Replace(PathUtil.BuildResourcesPath, "").ToLower();
            assetBundle.assetBundleName = bundlePath + ".ab";
            assetBundleBuilds.Add(assetBundle);

            string bundleInfo = assetPath + "|" + bundlePath + ".ab";
            List<string> dependenceInfo = GetDependencies(assetPath);
            if (dependenceInfo.Count > 0)
                bundleInfo = bundleInfo + "|" + string.Join("|", dependenceInfo);
            bundleInfos.Add(bundleInfo);
        }

        if(Directory.Exists(PathUtil.BuildOutPath))
            Directory.Delete(PathUtil.BuildOutPath, true);
        Directory.CreateDirectory(PathUtil.BuildOutPath);

        BuildPipeline.BuildAssetBundles(PathUtil.BuildOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);

        File.WriteAllLines(PathUtil.BuildOutPath + "/" + AppConst.FileListName, bundleInfos);
        AssetDatabase.Refresh();
    }

    static List<string> GetDependencies(string curFire)
    {
        List<string> denpendencies = new List<string>();
        string[] files = AssetDatabase.GetDependencies(curFire);
        denpendencies = files.Where(file => !file.EndsWith(".cs") && !file.Equals(curFire)).ToList();
        return denpendencies;
    }
}
