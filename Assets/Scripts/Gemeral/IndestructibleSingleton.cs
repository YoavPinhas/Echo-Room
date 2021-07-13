using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndestructibleSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        Instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(Instance);
        OnAwake();
    }
    protected virtual void OnAwake() { }


}
