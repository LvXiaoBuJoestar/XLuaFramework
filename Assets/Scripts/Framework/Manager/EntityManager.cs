using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    Dictionary<string, GameObject> m_Entities = new Dictionary<string, GameObject>();
    Dictionary<string, Transform> m_Groups = new Dictionary<string, Transform>();

    private Transform parent;

    private void Awake()
    {
        parent = transform.parent.Find("Entity");
    }

    public void SetEntityGroup(List<string> groups)
    {
        for (int i = 0; i < groups.Count; i++)
        {
            GameObject go = new GameObject("Group-" + groups[i]);
            go.transform.SetParent(parent, false);
            m_Groups[groups[i]] = go.transform;
        }
    }

    Transform GetGroup(string group)
    {
        if (!m_Groups.ContainsKey(group))
            Debug.LogError("group is not existed");
        return m_Groups[group];
    }

    public void ShowEntity(string name, string group, string luaName)
    {
        GameObject entity = null;
        if(m_Entities.TryGetValue(name, out entity))
        {
            EntityLogic logic = entity.GetComponent<EntityLogic>();
            logic.OnShow();
            return;
        }

        Manager.ResourceManager.LoadPrefab(name, (UnityEngine.Object obj) =>
        {
            entity = Instantiate(obj) as GameObject;
            entity.transform.SetParent(GetGroup(group));
            m_Entities[name] = entity;

            EntityLogic entityLogic = entity.AddComponent<EntityLogic>();
            entityLogic.Init(luaName);
            entityLogic.OnShow();
        });
    }
}
