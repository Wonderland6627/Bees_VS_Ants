using System;
using UnityEngine;

public class SingleMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (typeof(T))
                {
                    if (instance == null)
                    {
                        instance = FindObjectOfType<T>();
                        if (instance == null)
                        {
                            GameObject go = new GameObject();
                            instance = go.AddComponent<T>();
                            go.name = typeof(T).Name;

                            DontDestroyOnLoad(go);
                        }
                    }
                }
            }

            return instance;
        }
    }
}