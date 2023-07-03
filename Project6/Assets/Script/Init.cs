using BestHTTP;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Init : SingletonMono<Init>
{
    private void Awake()
    {
        
    }
    void Start()
    {
        UIMgr.Instance.ShowUI(nameof(LoginPanel),1);
        UIMgr.Instance.ShowUI(nameof(TipPanel), 2);
    }

    IEnumerator GetVersionInfo()
    {
        var uri = "http://1.13.174.102:9090/version.ini";
        UnityWebRequest req = UnityWebRequest.Get(uri);
        yield return req.SendWebRequest();
        Debug.Log("Server return:" + req.downloadHandler.text);

        req.Dispose();
        yield break;
    }

    IEnumerator DownloadAndSaveBinFile()
    {
        var uri = "http://1.13.174.102:9090/1414432983.png";
        UnityWebRequest req = UnityWebRequest.Get(uri);
        yield return req.SendWebRequest();
        //GameUtility.SafeWriteAllBytes(outputFileName, req.downloadHandler.data);
        // end
        if (req.isNetworkError || req.isHttpError)
        {
            Debug.Log(req.error);
        }
        else
        {
            Debug.Log(req.downloadHandler.data);
        }
        req.Dispose();
        yield break;
    }
}
