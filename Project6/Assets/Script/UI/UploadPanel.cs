using BestHTTP;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UploadPanel : MonoBehaviour
{
    public Button BackBtn;
    public Button UploadBtn;
    public Button AddImg;
    public Button AddApp;
    public InputField InputField;//游戏名
    public InputField InputField2;//游戏包名
    public InputField InputField3;//游戏介绍
    public Dropdown GameTypeDropdown;//游戏类型
    public Image Image;
    public Text AppPath;
    public Text GameTypetext;
    private string ImgPath;
    public Slider ProgressSlider;
    public Text ProgressText;
    private bool isAdd;
    private string GUID;

    [Obsolete]
    private void Awake()
    {
        BackBtn.onClick.AddListener(() => {
            if(isAdd)
                Init.Instance.StartCoroutine(MainPanel.Instance.ShowAllGame());
            UIMgr.Instance.HideUI(nameof(UploadPanel));
        });
        UploadBtn.onClick.AddListener(() => { UploadBtnOnclick(); });
        AddImg.onClick.AddListener(() => { AddImage(); });
        AddApp.onClick.AddListener(() => { AddAPP();  });
    }
    private void OnEnable()
    {
        isAdd = false;
        Image.sprite = null;
        InputField.text = "";
        ImgPath = "";
        AppPath.text = "";
        ProgressSlider.gameObject.SetActive(false);
    }
    [Obsolete]
    private void UploadBtnOnclick()
    {
        if (InputField.text == "")
        {
            TipPanel.Instance.ShowMsg("游戏名不能为空");
            return;    
        }
        if (InputField2.text == "")
        {
            TipPanel.Instance.ShowMsg("App包名不能为空");
            return;
        }
        if (ImgPath == "")
        {
            TipPanel.Instance.ShowMsg("图片不能为空");
            return;
        }
        if (AppPath.text == "")
        {
            TipPanel.Instance.ShowMsg("App不能为空");
            return;
        }
        if (InputField3.text == "")
        {
            TipPanel.Instance.ShowMsg("游戏介绍不能为空");
            return;
        }
        GUID = GetGUID();
        StartCoroutine(Upload());           
      }
    [Obsolete]
    private IEnumerator Upload()
    {
        ProgressSlider.gameObject.SetActive(true);
        yield return StartCoroutine(GetName(GUID + ".apk"));
        yield return StartCoroutine(UploadFileToServer());

        isAdd = true;
    }
    [Obsolete]
    IEnumerator GetName(string FileName)
    {
        //服务器地址
        string url = "http://1.13.174.102:9090/";
        //get请求的接口
        string getInterface = "Login";
        //传给服务器的数据，？开头，每条数据以key=value的形式用&拼接起来
        string getParams = $"?uname={FileName}&pwd=123456";
        WWW _www = new WWW(url + getInterface + getParams);
        yield return _www;
        if (_www.isDone && string.IsNullOrEmpty(_www.error))
        {
            //服务器传过来的数据
            Debug.Log(_www.text);
            Upload();
        }
        else
        {
            Debug.Log(_www.error);
        }
    }
    [Obsolete]
    IEnumerator UploadFileToServer()
    {
        byte[] fileDatas = HttpClient.Instance.FileContent(AppPath.text);
        var uri = "http://1.13.174.102:9090/UploadImgFile";
        UnityWebRequest req = UnityWebRequest.Put(uri, fileDatas);
        req.SendWebRequest();
        while (!req.isDone)
        {
            ProgressSlider.value = req.uploadProgress;
            yield return null;
        }

        if (req.isNetworkError || req.isHttpError)
        {
            Debug.Log(req.error);
        }
        else
        {
            yield return StartCoroutine(GetName(GUID + ".png"));
            yield return StartCoroutine(UploadImageToServer());
            ProgressSlider.gameObject.SetActive(false);
        }

        req.Dispose();
        yield break;
    }

    IEnumerator UploadImageToServer()
    {
        byte[] fileDatas = HttpClient.Instance.FileContent(ImgPath);
        var uri = "http://1.13.174.102:9090/UploadImgFile";
        UnityWebRequest req = UnityWebRequest.Put(uri, fileDatas);
        yield return req.SendWebRequest();

        GameTypetext.text = GameTypeDropdown.options[GameTypeDropdown.value].text;

        var mySqlTools = new SqlHelper();
        mySqlTools.Open();
        mySqlTools.InsertInto("imagelist", new[] { "GUID", "ByUser","GameName", "PackageName", "Introduction","GameType","HotCount", "LowCount" }, new[] { GUID, LoginPanel.UserID.ToString(), InputField.text, InputField2.text, InputField3.text, GameTypetext.text, "0" , "0" });
        mySqlTools.Close();
        req.Dispose();
        MainPanel.updateflag_for_ShowGame = true;
        yield break;
    }

    ///// <summary>
    ///// 上传图片
    ///// </summary>
    ///// <returns></returns>
    //public IEnumerator UploadImage()
    //{
    //    byte[] data = HttpClient.Instance.FileContent(ImgPath);

    //    string serverUrl = "http://1.13.174.102:80/index.php";
    //    var request = new HTTPRequest(new System.Uri(serverUrl), HTTPMethods.Post, OnFinishedImage);
    //    request.AddField("folder", "upload/");
    //    request.AddBinaryData("Pic", data, GUID + ".png", "image/png");
    //    request.SetHeader("Content-Type", "application/octet-stream");
    //    request.DisableCache = true;
    //    request.Send();
    //    yield return null;
    //}
    //void OnFinishedImage(HTTPRequest originalRequest, HTTPResponse response)
    //{
    //    Debug.Log("finished...");
    //    Debug.Log("Response:" + response.DataAsText);
    //    var mySqlTools = new SqlHelper();
    //    mySqlTools.Open();
    //    mySqlTools.InsertInto("imagelist", new[] { "GUID", "GameName", "PackageName" }, new[] { GUID, InputField.text, InputField2.text });
    //    mySqlTools.Close();
    //}


    //public IEnumerator UploadApp(string fileName, byte[] data)
    //{
    //    ProgressSlider.gameObject.SetActive(true);
    //    ProgressSlider.maxValue = 100;
    //    ProgressSlider.value = 0;
    //    string serverUrl = "http://1.13.174.102:80/index.php";
    //    var request = new HTTPRequest(new System.Uri(serverUrl), HTTPMethods.Post, OnFinished);
    //    request.AddField("folder", "upload/");
    //    request.AddBinaryData("Pic", data, fileName, "image/png");
    //    request.SetHeader("Content-Type", "application/octet-stream");
    //    request.OnUploadProgress = OnUploadProgress;
    //    request.DisableCache = true;
    //    request.Send();
    //    yield return null;
    //}
    //void OnUploadProgress(HTTPRequest request, long uploaded, long length)
    //{
    //    float progressPercent = (uploaded / (float)length) * 100.0f;
    //    ProgressSlider.value= progressPercent;
    //}
    //void OnFinished(HTTPRequest originalRequest, HTTPResponse response)
    //{
    //    Debug.Log("finished...");
    //    Debug.Log("Response:" + response.DataAsText);

    //    //byte[] data = HttpClient.Instance.FileContent(ImgPath);
    //    //StartCoroutine(UploadImage(GUID + ".png", data));
    //}

    [Obsolete]
    private void AddImage()
    {
        var file = ResLoadHelper.Instance.GetFile(ResType.Texture);
        ImgPath = file.Item1;
        StartCoroutine(Load(file.Item1, Image));
    }

    private void AddAPP()
    {
        var file = ResLoadHelper.Instance.GetFile(ResType.App);
        AppPath.text = file.Item1;
    }

    public static string GetGUID()
    {
        long a = UnityEngine.Random.Range(100000000, 999999999);
        long b = UnityEngine.Random.Range(100000000, 999999999);
        long c = UnityEngine.Random.Range(100000000, 999999999);
        return (a+b+c).ToString();
    }

    [Obsolete]
    /// <summary>
    /// 使用www加载
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator Load(string url, Image image)
    {
        double startTime = (double)Time.time;
        //请求WWW
        WWW www = new WWW(url);

        yield return www;
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            //获取Texture
            Texture2D texture = www.texture;
            Debug.Log(image);
            Debug.Log(texture);
            //创建Sprite
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            startTime = (double)Time.time - startTime;
            Debug.Log("www加载用时 ： " + startTime);

        }
    }
    

 



}
