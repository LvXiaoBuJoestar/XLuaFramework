using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    Dictionary<string, Transform> m_UIGroup = new Dictionary<string, Transform>();

    private Transform m_UIParent;

    private void Awake()
    {
        m_UIParent = transform.parent.Find("UI");
    }

    public void SetUIGroup(List<string> group)
    {
        for(int i = 0; i < group.Count; i++)
        {
            GameObject go = new GameObject("Group-" + group[i]);
            go.transform.SetParent(m_UIParent, false);
            m_UIGroup[group[i]] = go.transform;
        }
    }

    Transform GetUIGroup(string group)
    {
        if (!m_UIGroup.ContainsKey(group))
            Debug.LogError("group is not existed");
        return m_UIGroup[group];
    }

    public void OpenUI(string uiName, string group, string luaName)
    {
        GameObject ui = null;
        Transform parent = GetUIGroup(group);

        string uiPath = PathUtil.GetUIPath(uiName);
        Object obj = Manager.PoolManager.Spawn("UI", uiPath);

        if(obj != null)
        {
            ui = obj as GameObject;
            UILogic uILogic = ui.GetComponent<UILogic>();
            uILogic.OnOpen();
            ui.transform.SetParent(parent, false);
            return;
        }

        Manager.ResourceManager.LoadUI(uiName, (UnityEngine.Object obj) =>
        {
            ui = Instantiate(obj) as GameObject;
            ui.transform.SetParent(parent, false);
            UILogic uILogic = ui.AddComponent<UILogic>();
            uILogic.AssetName = uiPath;
            uILogic.Init(luaName);
            uILogic.OnOpen();
        });
    }
}
