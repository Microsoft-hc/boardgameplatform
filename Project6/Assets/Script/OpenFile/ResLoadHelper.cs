
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Text;
using UnityEngine.Video;

public class ResLoadHelper : Singleton<ResLoadHelper>
{
    public string CopyPath = Application.streamingAssetsPath + "/Res/";

    public override void Init()
    {
        base.Init();
    }

    /// <summary>
    /// 添加资源进游戏---保存资源路径进json
    /// </summary>
    public (string,string) GetFile(ResType ResType)
    {
        OpenFileName openFileName = new OpenFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);
        //openFileName.filter = "Excel文件(*.xlsx)\0*.xlsx";
        openFileName.file = new string(new char[256]);//文件原始路径，格式（D:\3-图片-花.jpg）
        //openFileName.filter = "图片文件(*jpg;*.png)\0*.jpg;*.png\0视频文件(*.mp3)\0*.mp3\0\0";
        openFileName.filter = GetFileType(ResType);
        openFileName.maxFile = openFileName.file.Length;
        openFileName.fileTitle = new string(new char[64]);//文件名字(带格式后缀)，格式（3-图片-花.jpg）
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        openFileName.title = "窗口标题";
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        string url = string.Empty;
        if (LocalDialog.GetSaveFileName(openFileName))
        {
            var NewName = openFileName.fileTitle.Split('.');
            url = openFileName.file.Replace("\\", "/");
            Debug.Log(openFileName.fileTitle);
            string ResName=string.Empty;
            //switch (ResType)
            //{
            //    case ResType.Audio:
            //        var ResEntity = GameSession.Current.UserData.GetAudioEntityList().Find(o => o.ResName == UID + "." + NewName[1] && o.Type == Type1);
            //        if (ResEntity != null) return string.Empty;
            //        var Entity1= GameSession.Current.UserData.AddAudioEntity("", NewName[0], Type1, url);
            //        ResName = Entity1.ResName;
            //        break;
            //    case ResType.Texture:
            //        var Entity2 = GameSession.Current.UserData.AddTextureEntity("", Type1, Type2, url);
            //        ResName = Entity2.ResName;
            //        break;
            //    case ResType.Video:
            //        var Entity3 = GameSession.Current.UserData.AddTextureEntity("", Type1, Type2, url);
            //        ResName = Entity3.ResName;
            //        break;
            //}
            //FileCopy(url, CopyPath + ResName);
            return (url, openFileName.fileTitle);
        }
        return ("", "");
    }

    /// <summary>
    /// 获取文件类型
    /// </summary>
    private string GetFileType(ResType ResType)
    {
        switch (ResType)
        {
            case ResType.Audio:
                return "音乐文件(*mp3;*.WAV*.OGG)\0*.mp3;*.WAV*.OGG\0\0";
            case ResType.Texture:
                return "图片文件(*jpg;*.png)\0*.jpg;*.png\0\0";
            case ResType.Video:
                return "视频文件(*mp4;*.3gp)\0*.mp4;*.3gp\0\0";
            case ResType.App:
                return "视频文件(*apk)\0*.apk\0\0";
            default:
                return string.Empty;
        }
    }


    ///// <summary>
    ///// 添加资源进游戏---保存资源路径进json
    ///// </summary>
    //public ResPathClass GetFileInPath(ResType ResType)
    //{
    //    OpenFileName openFileName = new OpenFileName();
    //    openFileName.structSize = Marshal.SizeOf(openFileName);
    //    openFileName.file = new string(new char[256]);//文件原始路径，格式（D:\3-图片-花.jpg）
    //    openFileName.filter = GetFileType(ResType);
    //    openFileName.maxFile = openFileName.file.Length;
    //    openFileName.fileTitle = new string(new char[64]);//文件名字(带格式后缀)，格式（3-图片-花.jpg）
    //    openFileName.maxFileTitle = openFileName.fileTitle.Length;
    //    openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
    //    openFileName.title = "窗口标题";
    //    openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

    //    ResPathClass ResPathClass = new ResPathClass();
    //    if (LocalDialog.GetSaveFileName(openFileName))
    //    {
    //        //Debug.Log(openFileName.fileTitle);
    //        var NewName = openFileName.fileTitle.Split('.');
    //        ResPathClass.ResPath = openFileName.file.Replace("\\", "/");
    //        ResPathClass.Name = NewName[0];
    //        ResPathClass.ResMD5 = ClassHelper.GetMD5(ResPathClass.ResPath) + "." + NewName[1];
    //        ResPathClass.resType = ResType;
    //        FileCopy(ResPathClass.ResPath, CopyPath + ResPathClass.ResMD5);
    //        return ResPathClass;
    //    }
    //    return null;
    //}


    private void FileCopy(string sourceFileName, string destFileName)
    {
        if (File.Exists(destFileName)) return;
        File.Copy(sourceFileName, destFileName);
    }




}

public enum ResType
{
    Audio,
    Texture,
    Video,
    App
}

