using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    [SerializeField] GameMode gameMode;

    private void Awake()
    {
        AppConst.GameMode = this.gameMode;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Manager.EventManager.Subscribe(10000, OnLuaInit);

        Manager.ResourceManager.ParseVersionFile();
        Manager.LuaManager.Init();
    }

    void OnLuaInit(object args)
    {
        Manager.LuaManager.StartLua("main");

        Manager.PoolManager.CreateGameObjectPool("UI", 10);
        Manager.PoolManager.CreateGameObjectPool("Monster", 120);
        Manager.PoolManager.CreateGameObjectPool("Effect", 120);
        Manager.PoolManager.CreateAssetPool("AssetBundle", 300);
    }

    private void OnApplicationQuit()
    {
        Manager.EventManager.UnSubscribe(10000, OnLuaInit);
    }
}
