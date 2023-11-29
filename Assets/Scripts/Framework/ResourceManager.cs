using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    internal class BundleInfo
    {
        public string AssetName;
        public string BundleName;
        public List<string> Dependencies;
    }

    Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();

    private void Start()
    {
        ParseVersionFile();
        LoadUI("TestUI", OnComplete);
    }

#if UNITY_EDITOR
    /// <summary>
    /// 编辑器环境下加载资源
    /// </summary>
    /// <param name="assetName">资源名</param>
    /// <param name="action">回调事件</param>
    void EditorLoadAsset(string assetName, Action<UnityEngine.Object> action = null)
    {
        UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, typeof(UnityEngine.Object));
        if (obj == null)
            Debug.LogErrorFormat("Asset:[{0}] is not existed", assetName);
        action?.Invoke(obj);
    }
#endif

    private void OnComplete(UnityEngine.Object @object)
    {
        GameObject go = Instantiate(@object, transform) as GameObject;
        go.transform.localPosition = Vector3.zero;
    }

    private void ParseVersionFile()
    {
        string versionFilePath = Path.Combine(PathUtil.BundleResourcePath, AppConst.FileListName);
        string[] data = File.ReadAllLines(versionFilePath);

        for (int i = 0; i < data.Length; i++)
        {
            BundleInfo bundleInfo = new BundleInfo();
            string[] info = data[i].Split('|');
            bundleInfo.AssetName = info[0];
            bundleInfo.BundleName = info[1];

            if (info.Length > 2)
            {
                bundleInfo.Dependencies = new List<string>(info.Length - 2);
                for (int j = 2; j < info.Length; j++)
                {
                    bundleInfo.Dependencies.Add(info[j]);
                }
            }

            m_BundleInfos.Add(bundleInfo.AssetName, bundleInfo);
        }
    }

    void LoadAsset(string assetName, Action<UnityEngine.Object> action)
    {
#if UNITY_EDITOR
        if (AppConst.GameMode == GameMode.EditorMode)
            EditorLoadAsset(assetName, action);
        else
#endif
            StartCoroutine(LoadBundleAsync(assetName, action));
    }
    IEnumerator LoadBundleAsync(string assetname, Action<UnityEngine.Object> action = null)
    {
        string bundleName = m_BundleInfos[assetname].BundleName;
        bundleName = Path.Combine(PathUtil.BundleResourcePath, bundleName);

        List<string> dependencies = m_BundleInfos[assetname].Dependencies;
        if(dependencies != null)
        {
            for (int i = 0; i < dependencies.Count;i++)
            {
                yield return LoadBundleAsync(dependencies[i]);
            }
        }

        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundleName);
        yield return request;
        AssetBundleRequest assetBundleRequest = request.assetBundle.LoadAssetAsync(assetname);
        yield return assetBundleRequest;

        action?.Invoke(assetBundleRequest?.asset);
    }

    public void LoadUI(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetUIPath(assetName), action);
    }
    public void LoadMusic(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetMusicPath(assetName), action);
    }
    public void LoadSound(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetSoundPath(assetName), action);
    }
    public void LoadEffect(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetEffectPath(assetName), action);
    }
    public void LoadScene(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetScenePath(assetName), action);
    }
}
