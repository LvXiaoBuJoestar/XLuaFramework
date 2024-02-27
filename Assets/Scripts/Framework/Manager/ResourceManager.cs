using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    class BundleInfo
    {
        public string AssetsName;
        public string BundleName;
        public List<string> Dependencies;
    }

    private Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();

    public void ParseVersionFile()
    {
        string url = Path.Combine(PathUtil.BundleResourcePath, AppConst.FileListName);
        string[] data = File.ReadAllLines(url);

        for (int i = 0; i < data.Length; i++)
        {
            BundleInfo bundleInfo = new BundleInfo();
            string[] infos = data[i].Split('|');
            bundleInfo.AssetsName = infos[0];
            bundleInfo.BundleName = infos[1];

            bundleInfo.Dependencies = new List<string>(infos.Length - 2);
            for (int j = 2; j < infos.Length; j++)
            {
                bundleInfo.Dependencies.Add(infos[j]);
            }
            m_BundleInfos.Add(bundleInfo.AssetsName, bundleInfo);
        }
    }

    IEnumerator LoadBundleAsync(string assetName, Action<UnityEngine.Object> action = null)
    {
        string bundleName = m_BundleInfos[assetName].BundleName;
        string bundlePath = Path.Combine(PathUtil.BundleResourcePath, bundleName);
        List<string> dependencies = m_BundleInfos[assetName].Dependencies;

        if(dependencies != null && dependencies.Count > 0)
        {
            for(int i = 0; i < dependencies.Count; i++)
            {
                yield return LoadBundleAsync(dependencies[i]);
            }
        }

        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return request;

        AssetBundleRequest bundleRequest = request.assetBundle.LoadAssetAsync(assetName);
        yield return bundleRequest;

        action?.Invoke(bundleRequest?.asset);

        Debug.Log("Package Mode Load Asset");
    }

    void EditorLoadAsset(string assetName, Action<UnityEngine.Object> action)
    {
#if UNITY_EDITOR
        Debug.Log("Editor Mode Load Asset");
        UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, typeof(UnityEngine.Object));
        if (obj == null)
            Debug.LogError("AssetName is not existed : " + assetName);
        action?.Invoke(obj);
#endif
    }

    private void LoadAsset(string assetName, Action<UnityEngine.Object> action)
    {
        if (AppConst.GameMode == GameMode.Editor)
            EditorLoadAsset(assetName, action);
        else
            StartCoroutine(LoadBundleAsync(assetName, action));
    }

    public void LoadUI(string assetName, Action<UnityEngine.Object> action = null) => LoadAsset(PathUtil.GetUIPath(assetName), action);
    public void LoadMusic(string assetName, Action<UnityEngine.Object> action = null) => LoadAsset(PathUtil.GetMusicPath(assetName), action);
    public void LoadSound(string assetName, Action<UnityEngine.Object> action = null) => LoadAsset(PathUtil.GetSoundPath(assetName), action);
    public void LoadEffect(string assetName, Action<UnityEngine.Object> action = null) => LoadAsset(PathUtil.GetEffectPath(assetName), action);
    public void LoadScene(string assetName, Action<UnityEngine.Object> action = null) => LoadAsset(PathUtil.GetScenePath(assetName), action);
}
