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

    class BundleData
    {
        public AssetBundle Bundle;
        public int Count;

        public BundleData(AssetBundle ab)
        {
            Bundle = ab;
            Count = 1;
        }
    }

    private Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();
    private Dictionary<string, BundleData> m_AssetBundles = new Dictionary<string, BundleData>();

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

            if (infos[0].IndexOf("LuaScripts") > 0)
                Manager.LuaManager.LuaNames.Add(infos[0]);
        }
    }

    BundleData GetBundle(string name)
    {
        BundleData bundle = null;
        if (m_AssetBundles.TryGetValue(name, out bundle))
        {
            bundle.Count++;
            return bundle;
        }
        return null;
    }

    IEnumerator LoadBundleAsync(string assetName, Action<UnityEngine.Object> action = null)
    {
        string bundleName = m_BundleInfos[assetName].BundleName;

        BundleData bundle = GetBundle(bundleName);

        if(bundle == null)
        {
            string bundlePath = Path.Combine(PathUtil.BundleResourcePath, bundleName);

            UnityEngine.Object obj = Manager.PoolManager.Spawn("AssetBundle", assetName);
            if(obj != null)
            {
                AssetBundle ab = obj as AssetBundle;
                bundle = new BundleData(ab);
            }
            else
            {
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
                yield return request;
                bundle = new BundleData(request.assetBundle);
            }

            m_AssetBundles[bundleName] = bundle;
        }

        List<string> dependencies = m_BundleInfos[assetName].Dependencies;
        if (dependencies != null && dependencies.Count > 0)
        {
            for (int i = 0; i < dependencies.Count; i++)
            {
                yield return LoadBundleAsync(dependencies[i]);
            }
        }

        if (assetName.EndsWith(".unity"))
        {
            action?.Invoke(null);
            yield break;
        }

        if (action == null)
            yield break;

        AssetBundleRequest bundleRequest = bundle.Bundle.LoadAssetAsync(assetName);
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

    public void UnLoadBundle(UnityEngine.Object obj)
    {
        AssetBundle ab = obj as AssetBundle;
        ab.Unload(true);
    }

    public void LoadUI(string assetName, Action<UnityEngine.Object> action = null) => LoadAsset(PathUtil.GetUIPath(assetName), action);
    public void LoadMusic(string assetName, Action<UnityEngine.Object> action = null) => LoadAsset(PathUtil.GetMusicPath(assetName), action);
    public void LoadSound(string assetName, Action<UnityEngine.Object> action = null) => LoadAsset(PathUtil.GetSoundPath(assetName), action);
    public void LoadEffect(string assetName, Action<UnityEngine.Object> action = null) => LoadAsset(PathUtil.GetEffectPath(assetName), action);
    public void LoadScene(string assetName, Action<UnityEngine.Object> action = null) => LoadAsset(PathUtil.GetScenePath(assetName), action);
    public void LoadLua(string assetName, Action<UnityEngine.Object> action = null) => LoadAsset(assetName, action);
    public void LoadPrefab(string assetName, Action<UnityEngine.Object> action = null) => LoadAsset(assetName, action);

    private void MinusOneBundleCount(string bundleName)
    {
        if(m_AssetBundles.TryGetValue(bundleName, out BundleData bundle))
        {
            if(bundle.Count > 0)
            {
                bundle.Count--;
                Debug.Log("bundle引用计数 : " + bundleName + " count : " + bundle.Count);
            }
            if(bundle.Count <= 0)
            {
                Debug.Log("放入bundle对象池 : " + bundleName);
                Manager.PoolManager.UnSpawn("AssetBundle", bundleName, bundle.Bundle);
                m_AssetBundles.Remove(bundleName);
            }
        }
    }

    public void MinusBundleCount(string assetName)
    {
        string bundleName = m_BundleInfos[assetName].BundleName;

        MinusOneBundleCount(bundleName);

        List<string> dependencies = m_BundleInfos[assetName].Dependencies;
        if(dependencies != null)
        {
            foreach(string dependence in dependencies)
            {
                string name = m_BundleInfos[dependence].BundleName;
                MinusOneBundleCount(name);
            }
        }
    }
}
