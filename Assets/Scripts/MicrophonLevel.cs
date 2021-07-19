using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#region Microphon Event for the Inspector
[System.Serializable]
public class MicrophonLevelEvent : UnityEngine.Events.UnityEvent<float> { }
#endregion
public class MicrophonLevel : SingletonMonoBehavior<MicrophonLevel>
{
    #region InspectorParameters
    [SerializeField] private bool debug = false;
    [SerializeField] private int sampleRate = 44100;
    [SerializeField, Range(0, 1)] private float consideredSilence = 0.6f;
    [SerializeField, Range(0,1)] private float consideredLoud = 0.03f;
    [SerializeField] private float secondsForYouCanDoBetter = 5;
    [SerializeField] private float secondsOfSilenceBeforNextScene = 5;
    [SerializeField] private float secondsBeforLoadingNextScene = 5;
    private float maxLoudnessThreshold = 0.7f;
    public MicrophonLevelEvent OnMicrophoneLevelCalculated;
    [SerializeField] private string lastSceneName = "EndScene";
    #endregion

    #region Private Parameters
    private string device;
    private AudioClip recordedClip;
    private int sampleScope = 128;
    private float waveLevel = 0;
    private float maxLevel = 0;
    private bool firstScream = true;
    private float counter = 0;
    private bool screamWasMade = false;
    #endregion

    #region Microphon Methods
   
    public void StartMicrophone()
    {
        recordedClip = Microphone.Start(null, true, 5, sampleRate);
        ArduinoMnager.Instance?.StartScreamLight();
        screamWasMade = false;
    }
    public void StopMicrophone()
    {
        Microphone.End(null);
        ArduinoMnager.Instance?.StopScreamLight();
        if(firstScream)
            firstScream = false;
    }

    private float GetRMS()
    {
        return GetRMSFromChannel(0) + GetRMSFromChannel(1);
    }

    private float GetRMSFromChannel(int channel)
    {
        int pos = Microphone.GetPosition(device) - (sampleScope - 1);
        if (pos < 0)
            return 0;
        float[] samples = new float[sampleScope];
        recordedClip.GetData(samples, channel);
        float sum = 0;
        foreach (float f in samples)
        {
            sum += f * f;
        }
        return Mathf.Sqrt(sum);
    }
    #endregion

    #region MonoBehavior
    private void OnDisable()
    {
        StopMicrophone();
    }
    private void OnApplicationFocus(bool focus)
    {
       StopMicrophone();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            StartMicrophone();  
        /*float min = 0f;
        float max = 10f;
        float level = Mathf.InverseLerp(min, max, GetRMS());
        if (level < threshold)
        {
            maxLevel = Mathf.Lerp(maxLevel, 0, slowingSpeed * Time.deltaTime);
        }
        else if (level < maxLevel)
            maxLevel = Mathf.Lerp(maxLevel, level, slowingSpeed * Time.deltaTime);
        else
            maxLevel = Mathf.Lerp(maxLevel, level, sensitivity * Time.deltaTime);
        level = Mathf.Lerp(level, 0, slowingSpeed * Time.deltaTime);
        OnMicrophoneLevelCalculated.Invoke(Mathf.Max(maxLevel, 0));*/
        float[] samples = new float[128];
        if(Microphone.GetPosition(null) <= 128)
            return;
        recordedClip.GetData(samples, Microphone.GetPosition(null)-128);
        maxLevel = Mathf.InverseLerp(consideredSilence, consideredLoud, Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples)));
        Debug.Log($"MaxLevel = {maxLevel}");
        OnMicrophoneLevelCalculated.Invoke(maxLevel);
        //if (debug)
        //    Debug.Log(maxLevel);
        
        if(!firstScream)
        {
            if(maxLevel > maxLoudnessThreshold)
            {
                screamWasMade = true;
                counter = 0;
            }
            if(screamWasMade && maxLevel < 0.35f)
                counter += Time.deltaTime;

            if(counter >= secondsOfSilenceBeforNextScene)
                StartCoroutine(GoToLastScene());
        }
    }

    private IEnumerator GoToLastScene()
    {
        StopMicrophone();
        yield return new WaitForSeconds(secondsBeforLoadingNextScene);
        SceneManager.LoadScene(lastSceneName);
    }
    #endregion
}
