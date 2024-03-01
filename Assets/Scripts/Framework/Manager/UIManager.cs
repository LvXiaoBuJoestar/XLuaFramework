using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    Dictionary<string, GameObject> m_UI = new Dictionary<string, GameObject>();

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
        if(m_UI.TryGetValue(uiName, out ui))
        {
            UILogic uILogic = ui.GetComponent<UILogic>();
            uILogic.OnOpen();
            return;
        }

        Manager.ResourceManager.LoadUI(uiName, (UnityEngine.Object obj) =>
        {
            ui = Instantiate(obj) as GameObject;
            ui.transform.SetParent(GetUIGroup(group), false);
            m_UI[uiName] = ui;
            UILogic uILogic = ui.AddComponent<UILogic>();
            uILogic.Init(luaName);
            uILogic.OnOpen();
        });
    }
}
