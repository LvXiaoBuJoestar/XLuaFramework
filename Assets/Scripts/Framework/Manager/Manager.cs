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

    private static UIManager uiManager;
    public static UIManager UIManager
    {
        get { return uiManager; }
    }

    private static EntityManager entityManager;
    public static EntityManager EntityManager
    {
        get { return entityManager; }
    }

    private static MySceneManager sceneManager;
    public static MySceneManager SceneManager
    {
        get { return sceneManager; }
    }

    private static SoundManager soundManager;
    public static SoundManager SoundManager
    {
        get { return soundManager; }
    }

    private static EventManager eventManager;
    public static EventManager EventManager
    {
        get { return eventManager; }
    }

    private void Awake()
    {
        resourceManager = gameObject.AddComponent<ResourceManager>();
        luaManager = gameObject.AddComponent<LuaManager>();
        uiManager = gameObject.AddComponent<UIManager>();
        entityManager = gameObject.AddComponent<EntityManager>();
        sceneManager = gameObject.AddComponent<MySceneManager>();
        soundManager = gameObject.AddComponent<SoundManager>();
        eventManager = gameObject.AddComponent<EventManager>();
    }
}
