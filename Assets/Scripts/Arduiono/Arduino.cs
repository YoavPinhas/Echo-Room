using System;
using System.Text;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
 using System.IO.Ports;
 using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public class ProximityEvent : UnityEngine.Events.UnityEvent{}
public class Arduino : MonoBehaviour
{
    private SerialPort serialPort = new SerialPort("COM4", 9600, Parity.None, 8);
    public ProximityEvent OnProximityDetected;
    public float secondsBeforeStartArduino = 30;
    private string startSceneName;
    private static Arduino instance;
    [HideInInspector]
    public bool arduinoExists = true;
    public static Arduino Instance => instance;
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
    private void Start()
    {
        startSceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(StartArduino());
    }

    void OnLevelWasLoaded()
    {
        if(SceneManager.GetActiveScene().name ==startSceneName)
        {
            LightOff();
            serialPort.Close();
            StartCoroutine(StartArduino());
        }
    }

    private void OnDisable()
    {
        LightOff();
        serialPort.Close();
    }

    private IEnumerator StartArduino()
    {
        yield return new WaitForSecondsRealtime(secondsBeforeStartArduino);
        serialPort.Open();
        serialPort.ReadTimeout = 10;
        serialPort.WriteTimeout = 100;
        
        StartCoroutine(WaitForProximityDection());
    }
    private IEnumerator WaitForProximityDection()
    {
        bool detected = false;
        string msg = "";
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        while(!detected){
            try
            {
                msg = serialPort.ReadLine();
                if(msg == "PROXIMITY_DETECTED")
                {
                    detected = true;
                    OnProximityDetected.Invoke();
                    yield break;
                }
            }
            catch
            {

            }
            yield return waitFrame;
        }
    }

    public void LightOn()
    {
        if(!serialPort.IsOpen)
            return;
        Debug.Log("Lights on");
        serialPort.WriteLine("light_on\r\n");
    }

    public void LightOff()
    {
        if(!serialPort.IsOpen)
            return;
        serialPort.WriteLine("light_off\r\n");
    }

    public void LightFadeIn()
    {
        if(!serialPort.IsOpen)
            return;
        serialPort.Write("fade_in\r\n");
    }

    public void LightFadeOut()
    {
        if(!serialPort.IsOpen)
            return;
        serialPort.Write("fade_out\r\n");
    }
}
