using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILogic : LuaBehaviour
{
    public string AssetName;

    Action m_LuaOpen;
    Action m_LuaClose;

    public override void Init(string luaName)
    {
        base.Init(luaName);
        m_ScriptEnv.Get("OnOpen", out m_LuaOpen);
        m_ScriptEnv.Get("OnClose", out m_LuaClose);
    }

    public void OnOpen()
    {
        m_LuaOpen?.Invoke();
    }

    public void Close()
    {
        m_LuaClose?.Invoke();
        Manager.PoolManager.UnSpawn("UI", AssetName, gameObject);
    }

    protected override void Clear()
    {
        base.Clear();
        m_LuaOpen = null;
        m_LuaClose = null;
    }
}
