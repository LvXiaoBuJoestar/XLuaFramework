using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaManager : MonoBehaviour
{
    public List<string> LuaNames = new List<string>();
    private Dictionary<string, byte[]> m_LuaScripts = new Dictionary<string, byte[]>();

    public LuaEnv LuaEnv;

    private void Awake()
    {
        LuaEnv = new LuaEnv();
        LuaEnv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);

        LuaEnv.AddLoader(Loader);
    }

    private void Update()
    {
        if (LuaEnv != null)
            LuaEnv.Tick();
    }

    private void OnDestroy()
    {
        if(LuaEnv != null)
        {
            LuaEnv.Dispose();
            LuaEnv = null;
        }
    }

    public void Init()
    {
        if (AppConst.GameMode == GameMode.Editor)
            EditorLoadLuaScript();
        else
            LoadLuaScript();
    }

    public void StartLua(string name)
    {
        LuaEnv.DoString(string.Format("require '{0}'", name));
    }

    byte[] Loader(ref string name)
    {
        return GetLuaScript(name);
    }

    public byte[] GetLuaScript(string name)
    {
        name = name.Replace('.', '/');
        string fileName = PathUtil.GetLuaPath(name);

        byte[] luaScript = null;
        if (!m_LuaScripts.TryGetValue(fileName, out luaScript))
            Debug.LogError("LuaScript is not existed : " + fileName);

        return luaScript;
    }

    void LoadLuaScript()
    {
        foreach(var name in LuaNames)
        {
            Manager.ResourceManager.LoadLua(name, (UnityEngine.Object obj) =>
            {
                AddLuaScripts(name, (obj as TextAsset).bytes);
                if(m_LuaScripts.Count >= LuaNames.Count)
                {
                    LuaNames.Clear();
                    LuaNames = null;
                    Manager.EventManager.Fire(10000);
                }
            });
        }
    }

    void AddLuaScripts(string assetName, byte[] luaScript)
    {
        m_LuaScripts[assetName] = luaScript;
    }

    void EditorLoadLuaScript()
    {
#if UNITY_EDITOR
        string[] luaFiles = Directory.GetFiles(PathUtil.LuaPath, "*.bytes", SearchOption.AllDirectories);
        for (int i = 0; i < luaFiles.Length; i++)
        {
            string fileName = PathUtil.GetStandardPath(luaFiles[i]);
            byte[] file = File.ReadAllBytes(fileName);
            AddLuaScripts(PathUtil.GetUnityPath(fileName), file);
        }
        Manager.EventManager.Fire(10000);
#endif
    }
}
