using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    Transform m_PoolParent;

    Dictionary<string, PoolBase> m_Pools = new Dictionary<string, PoolBase>();

    private void Awake()
    {
        m_PoolParent = transform.parent.Find("Pool");
    }

    private void CreatePool<T>(string poolName, float releaseTime) where T : PoolBase
    {
        if(!m_Pools.TryGetValue(poolName, out PoolBase pool))
        {
            GameObject go = new GameObject(poolName);
            go.transform.SetParent(m_PoolParent);
            pool = go.AddComponent<T>();
            pool.Init(releaseTime);
            m_Pools.Add(poolName, pool);
        }
    }

    public void CreateGameObjectPool(string poolName, float releaseTime)
    {
        CreatePool<GameObjectPool>(poolName, releaseTime);
    }

    public void CreateAssetPool(string poolName, float releaseTime)
    {
        CreatePool<AssetPool>(poolName, releaseTime);
    }

    public Object Spawn(string poolName, string assetName)
    {
        if(m_Pools.TryGetValue(poolName, out PoolBase pool))
        {
            return pool.Spawn(assetName);
        }
        return null;
    }

    public void UnSpawn(string poolName, string assetName, Object asset)
    {
        if(m_Pools.TryGetValue(poolName, out PoolBase pool))
        {
            pool.UnSpawn(assetName, asset);
        }
    }
}
