using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : SingletonMono<TipPanel>
{
    public Transform MsgObj;
    public Text MsgText;

    private void Awake()
    {
        MsgObj.gameObject.SetActive(false);
    }

    public void ShowMsg(string value)
    {
        StartCoroutine(Showmsh(value));
    }
    private IEnumerator Showmsh(string value)
    {
        MsgObj.gameObject.SetActive(true);
        MsgText.text = value;
        yield return new WaitForSeconds(1.5f);
        MsgObj.gameObject.SetActive(false);
    }

}
