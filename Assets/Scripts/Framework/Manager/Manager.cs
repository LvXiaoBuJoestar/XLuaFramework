using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static ResourceManager resourceManager;
    public static ResourceManager ResourceManager
    {
        get { return resourceManager; }
    }

    private static LuaManager luaManager;
    public static LuaManager LuaManager
    {
        get { return luaManager; }
    }

    private void Awake()
    {
        resourceManager = gameObject.AddComponent<ResourceManager>();
        luaManager = gameObject.AddComponent<LuaManager>();
    }
}
