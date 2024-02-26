using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildTool : Editor
{
    [MenuItem("Tools/Build Windows Bundle")]
    static void BuildWindowsBundle()
    {
        Build(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tools/Build Android Bundle")]
    static void BuildAndroidBundle()
    {
        Build(BuildTarget.Android);
    }

    static void Build(BuildTarget target)
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        List<string> bundleInfos = new List<string>();

        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);
        for(int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta")) continue;

            AssetBundleBuild assetBundle = new AssetBundleBuild();
            string fileName = PathUtil.GetStandardPath(files[i]);
            string assetName = PathUtil.GetUnityPath(fileName);
            assetBundle.assetNames = new string[] { assetName };
            string bundleName = fileName.Replace(PathUtil.BuildResourcesPath, "").ToLower();
            assetBundle.assetBundleName = bundleName + ".ab";

            assetBundleBuilds.Add(assetBundle);

            string[] dependenceInfo = GetDependencies(assetName);
            string bundleInfo = assetName + "|" + bundleName + ".ab";
            if (dependenceInfo.Length > 0)
                bundleInfo = bundleInfo + "|" + string.Join('|', dependenceInfo);

            bundleInfos.Add(bundleInfo);
        }

        if (Directory.Exists(PathUtil.BundleOutPath))
            Directory.Delete(PathUtil.BundleOutPath, true);
        Directory.CreateDirectory(PathUtil.BundleOutPath);

        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);
        File.WriteAllLines(PathUtil.BundleOutPath + "/" + AppConst.FileListName, bundleInfos);

        AssetDatabase.Refresh();
    }

    static string[] GetDependencies(string curFile)
    {
        string[] files = AssetDatabase.GetDependencies(curFile);
        return files.Where(file => !file.EndsWith(".cs") && !file.Equals(curFile)).ToArray();
    }
}
