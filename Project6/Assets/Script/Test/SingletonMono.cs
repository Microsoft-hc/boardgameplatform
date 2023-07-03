using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoDontDestroy<T> : MonoBehaviour where T : SingletonMonoDontDestroy<T>
{
    
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject managerGo = new GameObject(typeof(T).Name);
                    _instance = managerGo.AddComponent<T>();
                    DontDestroyOnLoad(managerGo);
                }
                _instance.Init();
            }
            return _instance;
        }
    }

    public virtual void Init() { }
}

public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject managerGo = new GameObject(typeof(T).Name);
                    _instance = managerGo.AddComponent<T>();
                }
            }
            _instance.Init();
            return _instance;
        }
    }

    public virtual void Init() { }
}


public class Singleton<T> where T : class, new()
{
    private static T _current;
    /// <summary>
    /// 当前实例
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_current == null)
            {
                _current = new T();
            }
            return _current;
        }
    }

    protected Singleton()
    {
        if (_current != null)
        {

        }
        Init();
    }

    public virtual void Init() { }
}

