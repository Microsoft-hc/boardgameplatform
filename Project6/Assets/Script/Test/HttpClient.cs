using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.IO;
using System.Net;

public class HttpClient : SingletonMono<HttpClient>
{
    private string Url= "http://1.13.174.102:9090/";

    private void Start()
    {
        //string fileName = @"H:\APK.apk";
        //byte[] data = FileContent(fileName);
        ////UploadData("APK.apk", data);
        //DownloadPhoto("APK.apk");
    }

    public string GetUrl(HttpClsMessage MyHttpClsMessage)
    { 
        return Url + PackMessage(MyHttpClsMessage);
    }


    private void UploadData(string FileName, byte[] ImageBytes)
    {
        HttpClsMessage MyHttpClsMessage = new HttpClsMessage();
        MyHttpClsMessage.UploadFileName = FileName;
        MyHttpClsMessage.ComType = HttpCommandType.UploadFile;
        string AllHttpUrl = GetUrl(MyHttpClsMessage);

        var wc = new WebClient();
        wc.UploadDataAsync(new Uri(AllHttpUrl), ImageBytes);
        wc.UploadProgressChanged += new UploadProgressChangedEventHandler(UploadProgressCallback);
    }

    /// <summary>
    /// 下载服务器上的图片
    /// </summary>
    /// <param name="FileName">请求的服务器上图片的文件名,要加后缀.jpg</param>
    /// <returns>返回请求的Image对象</returns>
    public void DownloadPhoto(string FileName)
    {
        HttpClsMessage MyHttpClsMessage = new HttpClsMessage();
        MyHttpClsMessage.DownFileName = FileName;
        MyHttpClsMessage.ComType = HttpCommandType.DownloadFile;
        try
        {
            WebClient myClient = new WebClient();
            string AllHttpUrl = GetUrl(MyHttpClsMessage);

            var a= myClient.DownloadData(AllHttpUrl);
            Debug.LogError(a.Length);

            // 把 byte[] 写入文件
            FileStream fs = new FileStream(@"H:\111.apk", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(a);
            bw.Close();
            fs.Close();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

    }




    public byte[] FileContent(string fileName)
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

    private void UploadProgressCallback(object sender, UploadProgressChangedEventArgs e)
    {
        // Displays the operation identifier, and the transfer progress.
        Debug.LogFormat("{0}    uploaded {1} of {2} bytes. {3} % complete...",
            (string)e.UserState,
            e.BytesSent,
            e.TotalBytesToSend,
            e.ProgressPercentage);
    }
    private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
    {
        // Displays the operation identifier, and the transfer progress.
        Debug.LogFormat("{0}    downloaded {1} of {2} bytes. {3} % complete...",
            (string)e.UserState,
            e.BytesReceived,
            e.TotalBytesToReceive,
            e.ProgressPercentage);
    }



    private string End = "E-N-D";    //结束符
    private string StartV = "S-T-A-R-T";   //开始符
    private string PackMessage(HttpClsMessage MyHttpClsMessage)
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
}

public class HttpClsMessage
{
    public string SNNumber = "";

    public string ComType = "";
    public string Data = "";
    public string DownFileName = "";
    public string UploadFileName = "";

}

//固定的命令。
public class HttpCommandType
{
    public const string UploadFile = "UPLOADFILE";

    public const string DownloadFile = "DOWNFILE";

    public const string Login = "Login";


}