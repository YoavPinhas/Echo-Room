using System;
using System.Collections;
using UnityEngine;
using Google.Cloud.Speech.V1;
using System.IO;
using Grpc.Core;
using Grpc.Auth;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;

[Serializable]
public class OnFinalResultEvent : UnityEngine.Events.UnityEvent<string> { }

public class SpeechToText : MonoBehaviour
{
    #region Inspector Parameters
    [SerializeField] private string pathToApiKey;
    [SerializeField] private int maxRecordTimeInSeconds = 10;
    [SerializeField] private float timeGapBetweenWords = 0.7f;
    [SerializeField] private bool debug = false;
    [SerializeField] private float talktThreshold = 0.3f;
    [SerializeField] private OnFinalResultEvent OnFinalResult;
    #endregion

    private float timeCounter = 0;
    private float silenceTime = 0;

    private static SpeechToText instance;
    public static SpeechToText Instance => instance;
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
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToApiKey);
    }

    #region Constants
    private const float MICROPHONE_INITIALIZATION_TIMEOUT = 1;
    private const int SAMPLE_RATE = 16000;
    private Action<string> currentAction = null;
    #endregion
    #region Parameters
    private string microphoneDeviceName = null;
    private AudioClip recordedAudio;
    #endregion
    #region Microphone Methods
    private void StartMicrophone()
    {
        if (debug)
            Debug.Log("Initialize Microphone.");


        recordedAudio = Microphone.Start(null, false, maxRecordTimeInSeconds + 1, SAMPLE_RATE);

        //Wait for microphone initialization
        float timesStartTime = Time.realtimeSinceStartup;
        bool timedOut = false;
        //while (!(Microphone.GetPosition(microphoneDeviceName) > 0) && !timedOut)
        //{
        //    timedOut = Time.realtimeSinceStartup - timesStartTime >= MICROPHONE_INITIALIZATION_TIMEOUT;
        //}
        //if (timedOut)
        //{
        //    Debug.LogError("Unable to initialize microphone.", this);
        //    return;
        //}
    }
    private IEnumerator RecordingHandle()
    {
        bool userHasTalked = false;
        bool isLoud = false;
        silenceTime = 0;
        int start = 0;
        int end = 0;
        timeCounter = 0;
       
        if (debug)
            Debug.Log($"Start Recording. max recording seconds = {maxRecordTimeInSeconds}");
        while (Microphone.IsRecording(null))
        {
            if (timeCounter > maxRecordTimeInSeconds)
            {
                if (debug)
                    Debug.Log("Times up. Prepeare to stop recording.");
                if (userHasTalked)
                {
                    OnAudioRecorded(start, end);
                    yield break;
                }
                else
                {
                    currentAction.Invoke(null);
                    OnFinalResult.Invoke(null);
                    StopRecording();
                    yield break;
                }
            }
            else
            {
                isLoud = IsLoudPeack();
                if (!userHasTalked)
                {
                    if (isLoud)
                    {
                        userHasTalked = true;
                        timeCounter = 0;
                        silenceTime = 0;
                        start = Mathf.Clamp(Microphone.GetPosition(null) - 9000, 0, 99999999);
                        if (debug)
                            Debug.Log("User start talking.");
                    }
                }
                else
                {
                    
                    if (isLoud)
                        silenceTime = 0;
                    end = Microphone.GetPosition(null);// - (int)(timeGapBetweenWords);
                    if (silenceTime > timeGapBetweenWords)
                    {
                        OnAudioRecorded(start, end);
                    }
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
    private bool IsLoudPeack()
    {
        float[] waveData = new float[128];
        int micPosition = Microphone.GetPosition(null) - (128 + 1);
        if (micPosition < 0)
            return false;
        recordedAudio.GetData(waveData, micPosition);
        for (int i = 0; i < 128; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (wavePeak > talktThreshold)
            {
                return true;
            }
        }
        return false;
    }
    private void StopRecording()
    {
        if (debug)
            Debug.Log("Stop recording.");
        Microphone.End(microphoneDeviceName);
    }
    private void OnAudioRecorded(int start, int end)
    {
        StopRecording();
        if (start == end)
            return;
        AudioClip clip = ChopSound(recordedAudio ,start, end);
        if (debug)
            Debug.Log("Sending Audio To Google.");
        GetTextFromAudio(clip);
    }
    #endregion
    #region Google Speech-TO-Text Methods
    public void StartListening(Action<string> action = null, int maxSeconds = -1)
    {
        currentAction = action;
        if (maxSeconds > 0)
            maxRecordTimeInSeconds = maxSeconds;
        if (debug)
            Debug.Log("Prepare to start listening.");
        StartMicrophone();
        StartCoroutine(RecordingHandle());
    }
    private async void GetTextFromAudio(AudioClip clip)
    {
        if(!File.Exists(pathToApiKey))
        {
            Debug.LogError("Cannot Find ApiKey json file :(");
            return;
        }
        byte[] audio = AudioClip2Wave(clip);

        SpeechClient speech = SpeechClient.Create();
 


        RecognizeResponse response = await speech.RecognizeAsync(new RecognitionConfig()
        {
            Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
            SampleRateHertz = SAMPLE_RATE,
            LanguageCode = "en",
        }, RecognitionAudio.FromBytes(audio));
        if (response.Results != null && response.Results.Count > 0)
        {
            currentAction?.Invoke(response.Results[0].Alternatives[0].Transcript);
            if (debug)
                Debug.Log("Google think you said: " + response.Results[0].Alternatives[0].Transcript);
            OnFinalResult.Invoke(response.Results[0].Alternatives[0].Transcript);
        }
        else
        {
            currentAction?.Invoke(null);
            OnFinalResult.Invoke(null);
        }



    }
    #endregion
    #region MonoBehavior Methods
    
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            StopRecording();
    }
    private void OnDisable()
    {
        StopRecording();
    }
    
    #endregion
    #region Helper Methods
    Byte[] AudioClip2Wave(AudioClip clip) // Taks unity AudioClip and turns it into a  Wave File (Liner16 File)
    {
        var samples = new float[clip.samples];
        clip.GetData(samples, 0);

        Int16[] intData = new Int16[clip.samples];

        Byte[] byteData = new Byte[2 * clip.samples];

        int rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(byteData, i * 2);
        }
        return byteData;
    }

    private AudioClip ChopSound(AudioClip recordedAudio, int start, int end)
    {
        int frequency = recordedAudio.frequency;
        int samplesLength = end - start + 1;
        float[] clip_data = new float[recordedAudio.samples];
        float[] data = new float[samplesLength];
        var result = AudioClip.Create("Record", samplesLength, 1, frequency, false);

        recordedAudio.GetData(clip_data, 0);
        for (int i = start; i <= end; i++)
        {
            data[i - start] = clip_data[i];
        }
        result.SetData(data, 0);
        return result;
    }

    private void Update()
    {
        if (Microphone.IsRecording(null))
        {
            timeCounter += Time.deltaTime;
            silenceTime += Time.deltaTime;
        }
    }
    #endregion
}
