using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour
{
    static public T Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = GetComponent<T>();
            OnAwake();
        }
        else
            Destroy(gameObject);
    }
    protected virtual void OnAwake() { }
}
