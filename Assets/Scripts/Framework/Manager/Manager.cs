using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static ResourceManager resource;
    public static ResourceManager ResourceManager
    {
        get { return resource; }
    }

    private void Awake()
    {
        resource = gameObject.AddComponent<ResourceManager>();
    }
}
