using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaBehaviour : MonoBehaviour
{
    private LuaEnv m_LuaEnv = Manager.LuaManager.LuaEnv;
    protected LuaTable m_ScriptEnv;

    private Action m_LuaInit;
    private Action m_LuaUpdate;
    private Action m_LuaOnDestroy;

    public string luaName;

    private void Awake()
    {
        m_ScriptEnv = m_LuaEnv.NewTable();
        LuaTable meta = m_LuaEnv.NewTable();
        meta.Set("__index", m_LuaEnv.Global);
        m_ScriptEnv.SetMetaTable(meta);
        meta.Dispose();

        m_ScriptEnv.Set("self", this);
    }

    public virtual void Init(string luaName)
    {
        m_LuaEnv.DoString(Manager.LuaManager.GetLuaScript(luaName), luaName, m_ScriptEnv);
        m_ScriptEnv.Get("OnInit", out m_LuaInit);
        m_ScriptEnv.Get("Update", out m_LuaUpdate);

        m_LuaInit?.Invoke();
    }

    private void Update()
    {
        m_LuaUpdate?.Invoke();
    }

    private void OnDestroy()
    {
        m_LuaOnDestroy?.Invoke();
        Clear();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }

    protected virtual void Clear()
    {
        m_LuaOnDestroy = null;
        m_LuaInit = null;
        m_LuaUpdate = null;
        m_ScriptEnv?.Dispose();
        m_ScriptEnv = null;
    }
}
