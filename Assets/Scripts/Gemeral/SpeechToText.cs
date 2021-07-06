using System;
using System.Collections;
using UnityEngine;
using Google.Cloud.Speech.V1;
using System.IO;
using Grpc.Core;
using Grpc.Auth;
using Google.Apis.Auth.OAuth2;

[Serializable]
public class OnFinalResultEvent : UnityEngine.Events.UnityEvent<string> { }

public class SpeechToText : IndestructibleSingleton<SpeechToText>
{
    #region Inspector Parameters
    [SerializeField] private string pathToApiKey;
    [SerializeField] private int maxRecordTimeInSeconds = 10;
    [SerializeField] private float timeGapBetweenWords = 0.7f;
    [SerializeField] private bool debug = false;
    [SerializeField] private float talktThreshold = 0.3f;
    [SerializeField] private OnFinalResultEvent OnFinalResult;
    #endregion
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

        recordedAudio = Microphone.Start(microphoneDeviceName, false, maxRecordTimeInSeconds + 1, SAMPLE_RATE);

        //Wait for microphone initialization
        float timesStartTime = Time.realtimeSinceStartup;
        bool timedOut = false;
        while (!(Microphone.GetPosition(microphoneDeviceName) > 0) && !timedOut)
        {
            timedOut = Time.realtimeSinceStartup - timesStartTime >= MICROPHONE_INITIALIZATION_TIMEOUT;
        }
        if (timedOut)
        {
            Debug.LogError("Unable to initialize microphone.", this);
            return;
        }
    }
    private IEnumerator RecordingHandle()
    {
        bool userHasTalked = false;
        bool isLoud = false;
        float silenceTime = 0;
        int start = 0;
        int end = 0;
        float timeCounter = 0;
        if (debug)
            Debug.Log("Start Recording.");
        while (Microphone.IsRecording(microphoneDeviceName))
        {
            if (timeCounter > maxRecordTimeInSeconds)
            {
                if (debug)
                    Debug.Log("Times up. Prepeare to stop recording.");
                if (userHasTalked)
                    OnAudioRecorded(start, end);
                else
                    StopRecording();
                break;
            }
            else
            {
                isLoud = IsLoudPeack();
                if (!userHasTalked)
                {
                    if (isLoud)
                    {
                        userHasTalked = true;
                        start = Mathf.Clamp(Microphone.GetPosition(microphoneDeviceName) - 9000, 0, 99999999);
                        if (debug)
                            Debug.Log("User start talking.");
                    }
                }
                else
                {
                    timeCounter += Time.deltaTime;
                    if (!isLoud)
                        silenceTime += Time.deltaTime;
                    else
                        silenceTime = 0;
                    end = Microphone.GetPosition(microphoneDeviceName);// - (int)(timeGapBetweenWords);
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
        int micPosition = Microphone.GetPosition(microphoneDeviceName) - (128 + 1);
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
    public void StartListening(Action<string> action = null)
    {
        currentAction = action;
        if (debug)
            Debug.Log("Prepare to start listening.");
        StartMicrophone();
        StartCoroutine(RecordingHandle());
    }
    private void GetTextFromAudio(AudioClip clip)
    {
        if(!File.Exists(pathToApiKey))
        {
            Debug.LogError("Cannot Find ApiKey json file :(");
            return;
        }
        byte[] audio = AudioClip2Wave(clip);

    //    ChannelCredentials credential;
  //      credential = GoogleCredential.FromFile(pathToApiKey).ToChannelCredentials();

//        Channel channel = new Channel(SpeechClient.DefaultEndpoint.Host, SpeechClient.DefaultEndpoint.Port, credential);

        SpeechClient speech = SpeechClient.Create();

        RecognizeResponse response = speech.Recognize(new RecognitionConfig()
        {
            Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
            SampleRateHertz = SAMPLE_RATE,
            LanguageCode = "en",
        }, RecognitionAudio.FromBytes(audio));
        currentAction?.Invoke(response.Results[0].Alternatives[0].Transcript);
        if (debug)
            Debug.Log("Google think you said: " +response.Results[0].Alternatives[0].Transcript);
        OnFinalResult.Invoke(response.Results[0].Alternatives[0].Transcript);
    }
    #endregion
    #region MonoBehavior Methods
    protected override void OnAwake()
    {
        base.OnAwake();
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToApiKey);
    }
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
    #endregion
}
