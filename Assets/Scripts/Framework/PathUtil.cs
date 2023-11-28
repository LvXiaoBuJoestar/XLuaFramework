using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtil
{
    public static readonly string AssetsPath = Application.dataPath;

    public static readonly string BuildResourcesPath = AssetsPath + "/BuildResources/";
    public static readonly string BuildOutPath = Application.streamingAssetsPath;

    public static string GetUnityPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        return path.Substring(path.IndexOf("Assets"));
    }
    public static string GetStandardPath(string path)
    {
        if(string.IsNullOrEmpty(path))
            return string.Empty;
        return path.Trim().Replace("\\", "/");
    }
}
