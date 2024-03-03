using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    private string m_LogicName = "[SceneLogic]";

    private void Awake()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene scene1, Scene scene2)
    {
        if (!scene1.isLoaded || !scene2.isLoaded)
            return;

        SceneLogic logic1 = GetSceneLogic(scene1);
        SceneLogic logic2 = GetSceneLogic(scene2);

        logic1?.OnInActive();
        logic2?.OnActive();
    }

    public void SetActive(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
    }

    public void LoadScene(string sceneName, string luaName)
    {
        Manager.ResourceManager.LoadScene(sceneName, (UnityEngine.Object obj) =>
        {
            StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Additive));
        });
    }
    public void ChangeScene(string sceneName, string luaName)
    {
        Manager.ResourceManager.LoadScene(sceneName, (UnityEngine.Object obj) =>
        {
            StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Single));
        });
    }

    public void UnLoadScene(string sceneName)
    {
        StartCoroutine(UnLoadSceneAsync(sceneName));
    }

    bool IsLoadedScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene.isLoaded;
    }

    IEnumerator StartLoadScene(string sceneName, string luaName, LoadSceneMode mode)
    {
        if (IsLoadedScene(sceneName))
            yield break;

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, mode);
        async.allowSceneActivation = true;
        yield return async;

        Scene scene = SceneManager.GetSceneByName(sceneName);
        GameObject go = new GameObject(m_LogicName);

        SceneManager.MoveGameObjectToScene(go, scene);

        SceneLogic logic = go.AddComponent<SceneLogic>();
        logic.SceneName = sceneName;
        logic.Init(luaName);
        logic.OnEnter();
    }

    IEnumerator UnLoadSceneAsync(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded)
        {
            Debug.LogError("scene is not loaded");
            yield break;
        }

        SceneLogic logic = GetSceneLogic(scene);
        logic?.OnQuit();
        AsyncOperation async = SceneManager.UnloadSceneAsync(scene);
        yield return async;
    }

    private SceneLogic GetSceneLogic(Scene scene)
    {
        GameObject[] gameObjects = scene.GetRootGameObjects();
        foreach(GameObject go in gameObjects)
        {
            if(go.name.CompareTo(m_LogicName) == 0)
            {
                return go.GetComponent<SceneLogic>();
            }
        }
        return null;
    }
}
