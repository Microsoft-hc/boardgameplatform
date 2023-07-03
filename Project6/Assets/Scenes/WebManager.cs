using BestHTTP;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WebManager : MonoBehaviour
{

    private void Awake()
    {
    }


    public IEnumerator UploadFiles()
    {
        string serverUrl = "http://1.13.174.102:80/index.php";
        var request = new HTTPRequest(new System.Uri(serverUrl), HTTPMethods.Post, OnFinished);
        string fileName = @"H:\111.apk";
        byte[] data =HttpClient.Instance.FileContent(fileName);
        request.AddField("folder", "upload/");
        request.AddBinaryData("Pic", data, "111.apk", "image/png");
        request.SetHeader("Content-Type", "application/octet-stream");
        request.OnUploadProgress = OnUploadProgress;
        request.DisableCache = true;
        request.Send();
        yield return null;
    }
    void OnUploadProgress(HTTPRequest request, long uploaded, long length)
    {
        float progressPercent = (uploaded / (float)length) * 100.0f;
        Debug.Log("Uploaded: " + progressPercent.ToString("F2") + "%");
    }
    void OnFinished(HTTPRequest originalRequest, HTTPResponse response)
    {
        Debug.Log("finished...");
        Debug.Log("Response:" + response.DataAsText);
    }

    //进度条
    private Slider my_Slider;

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="url">下载的地址</param>
    /// <returns></returns>
    IEnumerator Download()
    {
        string url= "http://1.13.174.102:80/upload/333.png";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
        {
            print("当前的下载发生错误" + request.error);
            yield break;
        }
        while (!request.isDone)
        {
            print("当前的下载进度为：" + request.downloadProgress);
            yield return 0;
        }
        if (request.isDone)
        {
            //将下载的文件写入
            using (FileStream fs = new FileStream(@"H:\9.png", FileMode.Create))
            {
                byte[] data = request.downloadHandler.data;
                fs.Write(data, 0, data.Length);
            }
        }
    }



    private void OnGUI()
    {
        GUI.BeginGroup(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f - 100, 500, 200), "");

        GUI.Label(new Rect(10, 10, 400, 30), "1");

        if (GUI.Button(new Rect(10, 110, 150, 30), "上传 Image"))
        {
            StartCoroutine(UploadFiles());
        }

        if (GUI.Button(new Rect(10, 140, 150, 30), "下载 Image"))
        {
            StartCoroutine(Download());
        }
        GUI.EndGroup();
    }
}