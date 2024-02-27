using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class HotUpdate : MonoBehaviour
{
    byte[] m_ReadPathFileListData;
    byte[] m_ServerFileListData;

    internal class DownFileInfo
    {
        public string url;
        public string fileName;
        public DownloadHandler fileData;
    }

    IEnumerator DownloadFile(DownFileInfo info, Action<DownFileInfo> Complete)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(info.url);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("下载文件出错 : " + info.url + webRequest.error);
            yield break;
        }

        info.fileData = webRequest.downloadHandler;
        Complete?.Invoke(info);
        webRequest.Dispose();
    }

    IEnumerator DownloadFile(List<DownFileInfo> infos, Action<DownFileInfo> Complete, Action DownloadAllComplete)
    {
        foreach (var info in infos)
        {
            yield return DownloadFile(info, Complete);
        }
        DownloadAllComplete?.Invoke();
    }

    private List<DownFileInfo> GetFileList(string fileData, string path)
    {
        string content = fileData.Trim().Replace("\r", "");
        string[] files = content.Split('\n');
        List<DownFileInfo> downFileInfos = new List<DownFileInfo>();

        for (int i = 0; i < files.Length; i++)
        {
            string[] info = files[i].Split('|');
            DownFileInfo fileInfo = new DownFileInfo();
            fileInfo.fileName = info[1];
            fileInfo.url = Path.Combine(path, info[1]);
            downFileInfos.Add(fileInfo);
        }

        return downFileInfos;
    }

    private void Start()
    {
        if (IsFirstInstall())
            ReleaseResources();
        else
            CheckUpdate();
    }

    private void ReleaseResources()
    {
        string url = Path.Combine(PathUtil.ReadPath, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo() { url = url };
        StartCoroutine(DownloadFile(info, OnDownloadFileListComplete));
    }

    private void OnDownloadFileListComplete(DownFileInfo file)
    {
        m_ReadPathFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, PathUtil.ReadPath);
        StartCoroutine(DownloadFile(fileInfos, OnReleaseFileComplete, OnReleaseAllFileComplete));
    }

    private void OnReleaseFileComplete(DownFileInfo fileInfo)
    {
        Debug.Log("OnReleaseFileComplete:" + fileInfo.url);
        string writePath = Path.Combine(PathUtil.ReadWritePath, fileInfo.fileName);
        FileUtil.WriteFile(writePath, fileInfo.fileData.data);
    }

    private void OnReleaseAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ReadPathFileListData);
        CheckUpdate();
    }

    private void CheckUpdate()
    {
        string url = Path.Combine(AppConst.ResourcesUrl, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo() { url = url };
        StartCoroutine(DownloadFile(info, OnDownloadServerFileListComplete));
    }

    private void OnDownloadServerFileListComplete(DownFileInfo file)
    {
        m_ServerFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, AppConst.ResourcesUrl);
        List<DownFileInfo> downFileList = new List<DownFileInfo>();

        for (int i = 0; i < fileInfos.Count; i++)
        {
            string localFile = Path.Combine(PathUtil.ReadWritePath, fileInfos[i].fileName);
            if (!FileUtil.IsExisted(localFile))
            {
                fileInfos[i].url = Path.Combine(AppConst.ResourcesUrl, fileInfos[i].fileName);
                downFileList.Add(fileInfos[i]);
            }
        }

        if (downFileList.Count > 0)
            StartCoroutine(DownloadFile(downFileList, OnUpdateFileComplete, OnUpdateAllFilesComplete));
        else
            EnterGame();
    }

    private void OnUpdateFileComplete(DownFileInfo file)
    {
        Debug.Log("OnUpdateFileComplete:" + file.url);
        string writePath = Path.Combine(PathUtil.ReadWritePath, file.fileName);
        FileUtil.WriteFile(writePath, file.fileData.data);
    }

    private void OnUpdateAllFilesComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ServerFileListData);
        EnterGame();
    }

    private void EnterGame()
    {
        Manager.ResourceManager.ParseVersionFile();
        Manager.ResourceManager.LoadUI("TestUI", OnComplete);
    }

    private void OnComplete(UnityEngine.Object obj)
    {
        Instantiate(obj, transform);
    }

    private bool IsFirstInstall()
    {
        bool isExistsReadPath = FileUtil.IsExisted(Path.Combine(PathUtil.ReadPath, AppConst.FileListName));
        bool isExistsReadWritePath = FileUtil.IsExisted(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName));
        return isExistsReadPath && !isExistsReadWritePath;
    }
}
