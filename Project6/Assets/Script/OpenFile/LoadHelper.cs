using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class LoadHelper
{
    public static async Task<Sprite> LoadSpritePNG(string path)
    {
        var task = LoadAsyncSprite(path, ".png");
        Task<Sprite> t = await Task.WhenAny(task);
        return t.Result;
    }

    public static async Task<Sprite> LoadSpriteJPG(string path)
    {
        var task = LoadAsyncSprite(path, ".jpg");
        Task<Sprite> t = await Task.WhenAny(task);
        return t.Result;
    }

    static async Task<Sprite> LoadAsyncSprite(string url, string end)
    {
        string path = Path.Combine(Application.streamingAssetsPath, url + end);
        var getRequest = UnityWebRequest.Get(path);
        await getRequest.SendWebRequest();
        byte[] imgData = getRequest.downloadHandler.data;
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imgData);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot, 100.0f);
        return sprite;
    }

    /// <summary>
    /// 获取Texture2D
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task<Texture2D> LoadTexture2D(string path)
    {
        var task = LoadAsyncTexture2D(path);
        Task<Texture2D> t = await Task.WhenAny(task);
        return t.Result;
    }
    static async Task<Texture2D> LoadAsyncTexture2D(string path)
    {
        var getRequest = UnityWebRequest.Get(path);
        await getRequest.SendWebRequest();
        byte[] imgData = getRequest.downloadHandler.data;
        Texture2D tex = new Texture2D(50, 50);
        tex.LoadImage(imgData);
        return tex;
    }

    /// <summary>
    /// 获取音频
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task<AudioClip> LoadAudioClip(string path)
    {
        if (!GetFilePath(path)) return null;
        var task = LoadAsyncAudioClip(path);
        Task<AudioClip> t = await Task.WhenAny(task);
        return t.Result;
    }

    static async Task<AudioClip> LoadAsyncAudioClip(string path)
    {
        AudioClip clip = null;
        UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG);
        {
            await uwr.SendWebRequest();
            if (uwr.isNetworkError)
                Debug.LogError(uwr.error);
            else
            {
                clip = DownloadHandlerAudioClip.GetContent(uwr);
            }
        }
        return clip;
    }

    public static bool GetFilePath(string path)
    {
        if (!File.Exists(path))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 获取AssetBundle
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task<AssetBundle> LoadAssetBundle(string path)
    {
        //if (!File.Exists(path)) return null;
        var task = LoadAsset(path);
        Task<AssetBundle> t = await Task.WhenAny(task);
        return t.Result;
    }

    static async Task<AssetBundle> LoadAsset(string path)
    {
        AssetBundle ab = null;
        UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(path);
        {
            await uwr.SendWebRequest();
            if (uwr.isNetworkError)
                Debug.LogError(uwr.error);
            else
            {
                ab = (uwr.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
            }
        }
        return ab;
    }





}

public static class ExtensionMethods
{
    public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
    {
        var tcs = new TaskCompletionSource<object>();
        asyncOp.completed += obj => { tcs.SetResult(null); };
        return ((Task)tcs.Task).GetAwaiter();
    }
}