using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToLoopScene : MonoBehaviour
{
    private static GameObject instance;

    public string loopSceneName;
    public string startSceneName;

    public KeyCode keyToLoop;
    public KeyCode keyToStart;


    private void Awake()
    {
        if(instance != null && instance != gameObject)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = gameObject;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(keyToLoop))
        {
            SceneManager.LoadScene(loopSceneName);
        }
        if (Input.GetKeyDown(keyToStart))
        {
            SceneManager.LoadScene(startSceneName);
        }
    }
}
