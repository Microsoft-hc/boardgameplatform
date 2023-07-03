using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : SingletonMono<UIMgr>
{
    private Transform GameObj1;
    private Transform GameObj2;

    public Dictionary<string,GameObject> UIDic=new Dictionary<string,GameObject>();

    private void Awake()
    {
        GameObj1 = transform.Find("1");
        GameObj2 = transform.Find("2");
    }

    public void ShowUI(string UIName,int index)
    {

        GameObject ui = GetUI(UIName);
        if (ui == null)
        {
            ui =Instantiate(Resources.Load<GameObject>(UIName));
            RectTransform rectTransform = ui.GetComponent<RectTransform>();
            Transform tran = index == 1 ? GameObj1 : GameObj2;
            ui.transform.SetParent(tran);
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.offsetMax = new Vector2(0, 0);
            rectTransform.offsetMin = new Vector2(0, 0);
            ui.transform.localScale = Vector3.one;
            UIDic.Add(UIName,ui);
        }
        ui.SetActive(true);
    }

    public void HideUI(string UIName)
    {
        UIDic.TryGetValue(UIName, out var ui);
        ui.SetActive(false);
    }

    private GameObject GetUI(string UIName)
    {
        if (UIDic.ContainsKey(UIName))
            return UIDic[UIName];
        return null;
    }

}
