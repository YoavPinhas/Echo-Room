using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToLoopScene : MonoBehaviour
{
    private static GameObject instance;

    public string sceneName;
    public KeyCode key;

    private void Awake()
    {
        if(instance != null && instance != gameObject)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = gameObject;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
