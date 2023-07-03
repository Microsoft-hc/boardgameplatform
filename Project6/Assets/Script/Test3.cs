using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Test3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private IEnumerator Test()
    {
        string _uri = "http:/127.0.0.1:9090";
        using (UnityWebRequest unityWebRequest = new UnityWebRequest(_uri, UnityWebRequest.kHttpVerbPOST))
        {
            unityWebRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes("12345"))
            {
                contentType = "application/x-www-form-urlencoded"
            };

            unityWebRequest.timeout = 3;
            unityWebRequest.useHttpContinue = false;
            yield return unityWebRequest.SendWebRequest();
        }
    }



}
