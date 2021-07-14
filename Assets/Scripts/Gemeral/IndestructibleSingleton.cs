using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndestructibleSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance { get{return instance;}}

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = GetComponent<T>();
            DontDestroyOnLoad(gameObject);
            transform.parent = null;
            //DontDestroyOnLoad(Instance);
            OnAwake();
        }
    }
    protected virtual void OnAwake() { }

    void OnDestroy()
    {
        Debug.Log($"{gameObject.name} is destyoed");
        instance = null;
    }
}
