using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Microphon Event for the Inspector
[System.Serializable]
public class MicrophonLevelEvent : UnityEngine.Events.UnityEvent<float> { }
#endregion
public class MicrophonLevel : SingletonMonoBehavior<MicrophonLevel>
{
    #region InspectorParameters
    [SerializeField] private bool debug = false;
    [SerializeField] private int sampleRate = 44100;
    [SerializeField, Range(0, 1)] private float threshold = 0;
    [SerializeField, Range(0, 1)] private float sensitivity = 0;
    [SerializeField, Range(0, 5)] private float slowingSpeed = 1;
    public MicrophonLevelEvent OnMicrophoneLevelCalculated;
    #endregion

    #region Private Parameters
    private string device;
    private AudioClip recordedClip;
    private int sampleScope = 128;
    private float waveLevel = 0;
    private float maxLevel = 0;
    #endregion

    #region Microphon Methods
    private void InitMicrophone()
    {
        if (device == null)
            device = Microphone.devices[0];
        if (device == null && debug)
            Debug.LogError("Can't fine a connected microphone :(");
        if (debug)
            Debug.Log(device);

    }
    private void StartMicrophone()
    {
        recordedClip = Microphone.Start(device, true, 5, sampleRate);
    }
    private void StopMicrophone()
    {
        Microphone.End(device);
    }
    private float CalculateMaxLevel()
    {
        int pos = Microphone.GetPosition(device) - (sampleScope - 1);
        if (pos < 0)
            return 0;
        float[] waveData = new float[sampleScope];
        recordedClip.GetData(waveData, pos);
        float max = waveData.Max();
        return max * max;
    }
    #endregion

    #region MonoBehavior
    private void OnEnable()
    {
        InitMicrophone();
        StartMicrophone();
    }
    private void OnDisable()
    {
        StopMicrophone();
    }
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            InitMicrophone();
            StartMicrophone();
        }
        else
            StopMicrophone();
    }
    void Update()
    {
        float max = CalculateMaxLevel();
        if (max < threshold)
        {
            maxLevel = Mathf.Lerp(maxLevel, 0, slowingSpeed * Time.deltaTime);
        }
        else if (max < maxLevel)
            maxLevel = Mathf.Lerp(maxLevel, max, slowingSpeed * Time.deltaTime);
        else
            maxLevel = Mathf.Lerp(maxLevel, max, sensitivity);
        OnMicrophoneLevelCalculated.Invoke(maxLevel);
        if (debug)
            Debug.Log(maxLevel);
    }
    #endregion
}
