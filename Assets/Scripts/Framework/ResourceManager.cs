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
        LoadBundle("Assets/BuildResources/UI/Prefabs/Image.prefab", OnComplete);
    }

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

    void LoadBundle(string assetname, Action<UnityEngine.Object> action)
    {
        StartCoroutine(LoadBundleAsync(assetname, action));
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
}
