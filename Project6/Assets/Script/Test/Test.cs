using BestHTTP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class Test : MonoBehaviour
{

    private void Start()
    {
        //string fileName = @"H:\APK.apk";
        //byte[] data = FileContent(fileName);
        //var a= UploadPhoto("3.jpg", data);
        //Debug.Log(a);
        //StartCoroutine(UploadFiles2());
        //abc("APK.apk", data);
        UploadPost("", null);
        //StartCoroutine(UploadPost("",null));
    }

    private static string data;
    /// <summary>
    /// 获取数据
    /// </summary>
    /// <returns></returns>

    public string GetMyData()
    {
        Invoke();
        //new Task(() =>
        //{
        //    Invoke();
        //}).Start();
        //Console.WriteLine("我是主线程");

        if (data == null) return null;
        return data;
    }

    public static async Task<string> Invoke()
    {
        var result = GetPostLoad();
        data = await result;  //等待返回
        Console.WriteLine(data);  //输出返回
        return data;
    }

    public static async Task<string> GetPostLoad()
    {
        HttpWebRequest request = WebRequest.Create("http:/127.0.0.1:9090/test") as HttpWebRequest;
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";     //这里一定不要忘记了 我排查了好久 发现这里没有写这一句  弄的怀疑人生了 后来通过抓包对比 才发现这个差距  粗心了
        string data = "userId=123&agentId=456&companyId=789&versionTime=165665430918";
        byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);
        request.ContentLength = buf.Length;
        Stream newStream = request.GetRequestStream();
        newStream.Write(buf, 0, buf.Length);
        newStream.Close();
        HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
        string result = reader.ReadToEnd();
        return result;
    }

    public string UploadPost(string fileName, byte[] bytes)
    {
        string serverUrl = "http://127.0.0.1:9090/";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serverUrl);
        request.Method = "POST";
        request.Referer = fileName;
        request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
        string data = "userId=123&agentId=456&companyId=789&versionTime=165665430918";

        bytes = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);
        request.ContentLength = bytes.Length;
        Stream newStream = request.GetRequestStream();
        newStream.Write(bytes, 0, bytes.Length);

        Debug.Log(request) ;
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
        string retString = myStreamReader.ReadToEnd();
        Debug.Log(retString);
        myStreamReader.Close();
        newStream.Close();

        if (response != null)
        {
            response.Close();
        }
        if (request != null)
        {
            request.Abort();
        }
        return retString;
    }














    private void abc(string FileName, byte[] ImageBytes)
    {
        HttpClsMessage MyHttpClsMessage = new HttpClsMessage();
        MyHttpClsMessage.UploadFileName = FileName;
        MyHttpClsMessage.ComType = HttpCommandType.UploadFile;
        string AllHttpUrl = "http://127.0.0.1:5050/" + PackMessage(MyHttpClsMessage);

        var wc = new WebClient();
        wc.UploadDataAsync(new Uri(AllHttpUrl), ImageBytes);
        wc.UploadDataCompleted += Wc_DownloadDataCompleted;
    }

    private void Wc_DownloadDataCompleted(object sender, UploadDataCompletedEventArgs e)
    {
        Debug.Log(sender);
        Debug.Log(e.Result);
    }


    /// <summary>
    /// 上传图片到http服务器
    /// </summary>
    /// <param name="FileName">上传到服务器之后存储的图片的文件名,包含扩展名</param>
    /// <param name="ImageBytes">上传图片的字符组</param>
    /// 返回 0表示成功。
    public string UploadPhoto2(string FileName, byte[] ImageBytes)
    {
        string ReturnStr = "";

        HttpClsMessage MyHttpClsMessage = new HttpClsMessage();
        MyHttpClsMessage.UploadFileName = FileName;
        MyHttpClsMessage.ComType = HttpCommandType.UploadFile;

        try
        {
            WebClient myClient = new WebClient();
            string AllHttpUrl = "http://127.0.0.1:5050/" + PackMessage(MyHttpClsMessage);
            byte[] ReturnVlaue = myClient.UploadData(AllHttpUrl, ImageBytes); //取得返回值
            ReturnStr = ASCIIEncoding.ASCII.GetString(ReturnVlaue);

        }
        catch (Exception ex)
        {

            ReturnStr = ex.Message;
            return ReturnStr;
        }

        return ReturnStr;
    }


    /// <summary>
    /// 上传图片到http服务器
    /// </summary>
    /// <param name="FileName">上传到服务器之后存储的图片的文件名,包含扩展名</param>
    /// <param name="ImageBytes">上传图片的字符组</param>
    /// 返回 0表示成功。
    public string UploadPhoto(string FileName, byte[] ImageBytes)
    {
        string ReturnStr = "";

        HttpClsMessage MyHttpClsMessage = new HttpClsMessage();
        MyHttpClsMessage.UploadFileName = FileName;
        MyHttpClsMessage.ComType = HttpCommandType.UploadFile;

        try
        {
            WebClient myClient = new WebClient();
            string AllHttpUrl = "http://127.0.0.1:5050/" + PackMessage(MyHttpClsMessage);
            byte[] ReturnVlaue = myClient.UploadData(AllHttpUrl, ImageBytes); //取得返回值
            ReturnStr = ASCIIEncoding.ASCII.GetString(ReturnVlaue);

        }
        catch (Exception ex)
        {

            ReturnStr = ex.Message;
            return ReturnStr;
        }

        return ReturnStr;
    }
    private string End = "E-N-D";    //结束符
    private string StartV = "S-T-A-R-T";   //开始符
    public  string PackMessage(HttpClsMessage MyHttpClsMessage)
    {

        string All = StartV + "|"

                + MyHttpClsMessage.SNNumber +
            "|" + MyHttpClsMessage.ComType +
            "|" + MyHttpClsMessage.Data +
            "|" + MyHttpClsMessage.DownFileName +
            "|" + MyHttpClsMessage.UploadFileName

             + "|" + End;


        return All;

    }


    public IEnumerator UploadFiles2()
    {
        string serverUrl = "http://127.0.0.1:5050/";
        var request = new HTTPRequest(new System.Uri(serverUrl), HTTPMethods.Post, OnFinished);
        string fileName = @"H:\3.jpg";
        byte[] data = FileContent(fileName);
        //string fileName2 = @"E:\LocationSystem1.exe";
        //byte[] data2 = FileContent(fileName2);
        request.AddBinaryData("BigFile", data, "xxxttttt1.txt");
        //request.AddBinaryData("BigFile", data2, "xxxeeeeee2.exe");
        request.SetHeader("Content-Type", "application/octet-stream");
        request.OnUploadProgress = OnUploadProgress;
        request.DisableCache = true;
        request.Send();
        yield return null;
    }


    public IEnumerator UploadFiles()
    {
        string serverUrl = "http://127.0.0.1:7080/";
        var request = new HTTPRequest(new System.Uri(serverUrl), HTTPMethods.Post, OnFinished);
        string fileName = @"E:\xxx.txt";
        byte[] data = FileContent(fileName);
        string fileName2 = @"E:\LocationSystem1.exe";
        byte[] data2 = FileContent(fileName2);
        request.AddBinaryData("BigFile", data, "xxxttttt1.txt");
        request.AddBinaryData("BigFile", data2, "xxxeeeeee2.exe");
        request.SetHeader("Content-Type", "application/octet-stream");
        request.OnUploadProgress = OnUploadProgress;
        request.DisableCache = true;
        request.Send();
        yield return null;
    }

    void OnFinished(HTTPRequest originalRequest, HTTPResponse response)
    {
        Debug.Log("finished...");
        Debug.Log("Response:" + response.DataAsText);
    }

    void OnUploadProgress(HTTPRequest request, long uploaded, long length)
    {
        float progressPercent = (uploaded / (float)length) * 100.0f;
        Debug.Log("Uploaded: " + progressPercent.ToString("F2") + "%");
    }

    private byte[] FileContent(string fileName)
    {
        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            try
            {
                byte[] buffur = new byte[fs.Length];
                fs.Read(buffur, 0, (int)fs.Length);
                return buffur;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}


