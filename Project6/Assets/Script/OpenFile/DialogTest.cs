
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class DialogTest : MonoBehaviour
{
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 50), "Open"))
        {
            OpenFileName openFileName = new OpenFileName();
            openFileName.structSize = Marshal.SizeOf(openFileName);
            //openFileName.filter = "Excel文件(*.xlsx)\0*.xlsx";
            openFileName.file = new string(new char[256]);//文件原始路径，格式（D:\3-图片-花.jpg）
            openFileName.filter = "图片文件(*jpg;*.png)\0*.jpg;*.png\0视频文件(*.mp3)\0*.mp3\0\0";

            openFileName.maxFile = openFileName.file.Length;
            openFileName.fileTitle = new string(new char[64]);//文件名字(带格式后缀)，格式（3-图片-花.jpg）
            openFileName.maxFileTitle = openFileName.fileTitle.Length;
            openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
            openFileName.title = "窗口标题";
            openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

            if (LocalDialog.GetSaveFileName(openFileName))
            {
                Debug.Log(openFileName.file);
                Debug.Log(openFileName.fileTitle);

                string url = openFileName.file.Replace("\\", "/");
                StartCoroutine(DownloadAndSave(url, openFileName.fileTitle, null));
            }
        }
    }

    public static IEnumerator DownloadAndSave(string url, string name, Action<bool, string, byte[]> Finish = null)//路径，名字
    {
        url = Uri.EscapeUriString(url);
        //url = "D:/Others/Dr/Project/U3D/1.3System/SqlV1/Assets/StreamingAssets/1-视频-cat";
        Debug.Log(url);
        string Loading = string.Empty;
        bool b = false;
        WWW www = new WWW(url);
        if (www.error != null)
        {
            Debug.Log("error:" + www.error);
        }
        while (!www.isDone)
        {
            Debug.Log("没有完成");
            Loading = (((int)(www.progress * 100)) % 100) + "%";
            if (Finish != null)
            {
                Finish(b, Loading, null);
            }
            yield return 1;
        }
        if (www.isDone)
        {
            Debug.Log("完成了");
            //Loading = "100%";
            byte[] bytes = www.bytes;
            b = SaveAssets(Application.streamingAssetsPath + "/Resource", name, bytes);
            if (Finish != null)
            {
                //Finish(b, Loading);
                Finish(b, Loading, bytes);
            }
        }
    }

    public static bool SaveAssets(string path, string name, byte[] bytes)//（保存路径，名字）
    {
        Stream sw;
        FileInfo t = new FileInfo(path + "//" + name);//name需要为完整格式"DLAM.jpg"或者"DLAM.MP4"
        if (!t.Exists)
        {
            try
            {
                sw = t.Create();
                sw.Write(bytes, 0, bytes.Length);
                sw.Close();
                sw.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }
}
