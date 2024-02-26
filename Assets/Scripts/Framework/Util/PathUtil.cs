using UnityEngine;

public class PathUtil
{
    public static readonly string AssetsPath = Application.dataPath;

    public static readonly string BuildResourcesPath = AssetsPath + "/BuildResources/";

    public static readonly string BundleOutPath = Application.streamingAssetsPath;

    public static string BundleResourcePath
    {
        get
        {
            if (AppConst.GameMode == GameMode.Update)
                return Application.persistentDataPath;
            return Application.streamingAssetsPath;
        }
    }

    public static string GetUnityPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        return path.Substring(path.IndexOf("Assets"));
    }

    public static string GetStandardPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        return path.Trim().Replace("\\", "/");
    }

    public static string GetLuaPath(string name) => string.Format("Assets/BuildResources/LuaScripts/{0}.bytes", name);
    public static string GetUIPath(string name) => string.Format("Assets/BuildResources/UI/Prefabs/{0}.prefab", name);
    public static string GetMusicPath(string name) => string.Format("Assets/BuildResources/Audio/Music/{0}", name);
    public static string GetSoundPath(string name) => string.Format("Assets/BuildResources/Audio/Sound/{0}", name);
    public static string GetEffectPath(string name) => string.Format("Assets/BuildResources/Effect/Prefabs/{0}.prefab", name);
    public static string GetSpritePath(string name) => string.Format("Assets/BuildResources/Sprites/{0}", name);
    public static string GetScenePath(string name) => string.Format("Assets/BuildResources/Scenes/{0}.unity", name);
}
