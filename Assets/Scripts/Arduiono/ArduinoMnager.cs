using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArduinoMnager : MonoBehaviour
{
    public float secondsBetweenFades = 1;
    public string startSceneName = "OpeningScene";
    private bool listening = false;
    public void StartExperience() => SceneManager.LoadScene(startSceneName);

    private static ArduinoMnager instance;
    public static ArduinoMnager Instance => instance;
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void StartListeningLight()
    {
        listening = true;
        StartCoroutine(OnListeningLight());
    }

    void Update()
    {
       
    }
    public void StopListeningLight()
    {
        listening = false;
    }

    public void StartScreamLight() => Arduino.Instance.LightOn();
    public void StopScreamLight() => Arduino.Instance.LightFadeOut();
    private IEnumerator OnListeningLight()
    {
        WaitForSeconds wait = new WaitForSeconds(secondsBetweenFades);
        while(listening)
        {
            Arduino.Instance.LightFadeIn();
            yield return wait;
            Arduino.Instance.LightFadeOut();
            yield return wait;
        }
        Arduino.Instance.LightFadeOut();
    }
}
