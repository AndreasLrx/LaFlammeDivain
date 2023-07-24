using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance = null;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
            instance = GetComponent<T>();
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public static bool Instantiated()
    {
        return Instance != null;
    }

    public static void Destroy()
    {
        Destroy(Instance.gameObject);
        instance = null;
    }
}
