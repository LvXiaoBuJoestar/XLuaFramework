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
        Manager.ResourceManager.ParseVersionFile();
        Manager.LuaManager.Init(() => { Manager.LuaManager.StartLua("main"); });
    }
}
