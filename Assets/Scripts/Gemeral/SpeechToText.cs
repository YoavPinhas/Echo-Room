using UnityEngine;
using System.Collections;
using System.IO;
using IBM.Watson.SpeechToText.V1;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.DataTypes;
using System;

#region ApiKey and ServiceURL data structure
public struct APIFileData
{
    public string serviceURL;
    public string apiKey;
}
#endregion

[System.Serializable]
public class OnRecognitionEvent : UnityEngine.Events.UnityEvent<string> { }

public class SpeechToText : MonoBehaviour
{
    #region Inspector Parameters
    [SerializeField] private string pathToApiKey;
    [SerializeField] private int silenceSecondsToStopListening;
    public OnRecognitionEvent OnRecognition;

    #endregion

    #region Parameters
    APIFileData authenticationData;
    private int recordingRouting = 0;
    private string microphoneDevice = null;
    private AudioClip recordedAudio = null;
    private int recordingBufferSize = 1;
    private int recordingFrequency = 22050;
    private SpeechToTextService speechToTextService;
    private bool stopRecording = false;
    #endregion

    #region Init Methods
    private void GetAuthentication()
    {
        string json = null;
        try
        {
            json = File.ReadAllText(pathToApiKey);
            if (string.IsNullOrEmpty(json))
            {
                throw new FileLoadException($"Please Make sure the file in {pathToApiKey} has the right format!");
            }
        }
        catch (FileLoadException e)
        {
            Debug.LogError(e.Message);
        }
        
        try
        {
            authenticationData = JsonUtility.FromJson<APIFileData>(json);
            if(string.IsNullOrEmpty(authenticationData.apiKey) || string.IsNullOrEmpty(authenticationData.serviceURL))
                throw new IOException($"Please Make sure the file in {pathToApiKey} has the right format!");
        }
        catch(IOException e)
        {
            Debug.LogError(e.Message);
        }
    }
    private IEnumerator InitService()
    {
        IamAuthenticator authenticator = new IamAuthenticator(apikey: authenticationData.apiKey);

        //Wait for the Token Data
        while (!authenticator.CanAuthenticate())
            yield return null;
        speechToTextService = new SpeechToTextService(authenticator);
        speechToTextService.SetServiceUrl(authenticationData.serviceURL);
        speechToTextService.StreamMultipart = true;

        speechToTextService.RecognizeModel = "en-US_BroadbandModel";
        speechToTextService.DetectSilence = true;
        speechToTextService.EnableWordConfidence = true;
        speechToTextService.EnableTimestamps = false;
        speechToTextService.SilenceThreshold = 0.01f;
        speechToTextService.MaxAlternatives = 1;
        speechToTextService.EnableInterimResults = true; //Get only final result
        speechToTextService.OnError = OnError;
        speechToTextService.InactivityTimeout = silenceSecondsToStopListening;
        speechToTextService.ProfanityFilter = false;
        speechToTextService.SmartFormatting = true; //TODO: Check that one!
        speechToTextService.SpeakerLabels = false;
        speechToTextService.WordAlternativesThreshold = 0.3f; //TODO: Check that one also!
        speechToTextService.EndOfPhraseSilenceTime = null;

    }
    #endregion

    #region Speech Recognition Methods
    private void StartRecording()
    {
        if(recordingRouting == 0)
        {
            UnityObjectUtil.StartDestroyQueue();
            recordingRouting = Runnable.Run(RecordingHandler());
        }
    }
    private void StartListening()
    {
        stopRecording = false;
        speechToTextService.StartListening(OnRecognize, OnRecognizeSpeaker);
        StartRecording();
    }
    private void StopListening()
    {
        speechToTextService.StopListening();
        Microphone.End(microphoneDevice);
        Runnable.Stop(0);
        recordingRouting = 0;
        stopRecording = false;
    }
    private IEnumerator RecordingHandler()
    {
        recordedAudio = Microphone.Start(microphoneDevice, true, recordingBufferSize, recordingFrequency);
        yield return null; //Let recordingRouting get set

        if (recordedAudio == null)
        {
            StopRecording();
            yield break;
        }

        bool isFirstBlock = true;
        int midPoint = recordedAudio.samples / 2;
        float[] samples = null;
        while(recordingRouting != 0 && recordedAudio != null)
        {
            int writingPosition = Microphone.GetPosition(microphoneDevice);
            if(writingPosition > recordedAudio.samples || !Microphone.IsRecording(microphoneDevice))
            {
                Debug.LogError("Microphone disconnected.", this);
                StopRecording();
                yield break;
            }

            if (stopRecording)
            {
                StopListening();
                yield break;
            }

            if((isFirstBlock && writingPosition >= midPoint) || (!isFirstBlock && writingPosition < midPoint))
            {
                // front block is recorded, make a RecordClip and pass it onto our callback.
                samples = new float[midPoint];
                recordedAudio.GetData(samples, isFirstBlock ? 0 : midPoint);

                AudioData record = new AudioData();
                record.MaxLevel = Mathf.Max(Math.Abs(Mathf.Min(samples)), Mathf.Max(samples));
                record.Clip = AudioClip.Create("Recording", midPoint, recordedAudio.channels, recordingFrequency, false);
                record.Clip.SetData(samples, 0);
                speechToTextService.OnListen(record);
                isFirstBlock = !isFirstBlock;
            }
            else
            {
                // calculate the number of samples remaining until we ready for a block of audio, 
                // and wait that amount of time it will take to record.
                int remaining = isFirstBlock ? (midPoint - writingPosition) : (recordedAudio.samples - writingPosition);
                float timeRemaining = (float)remaining / (float)recordingFrequency;
                yield return new WaitForSeconds(timeRemaining);
            }
        }
        yield break;
    }

    private void StopRecording()
    {
        if(recordingRouting != 0)
        {
            Microphone.End(microphoneDevice);
            Runnable.Stop(recordingRouting);
            recordingRouting = 0;
        }
    }

    private void OnError(string error)
    {
        speechToTextService.StopListening();
        Debug.LogError($"Error has occurred in SpeechToText. error: {error}");
    }
    private void OnRecognize(SpeechRecognitionEvent results)
    {
        if (results == null || results.results.Length == 0)
            return;
        Debug.Log("On Recognize:");
        string res = "";
        if (results.HasFinalResult())
        {
            foreach(var result in results.results)
            {
                if (result.final)
                {
                    res = result.alternatives[0].transcript;
                    break;
                }
            }
            StopListening();
            OnRecognition.Invoke(res);
        }
        /*foreach(var result in results.results)
        {
            foreach(var alternative in result.alternatives)
            {
                if()
                string text = string.Format("{0} ({1}, {2:0.00})\n", alternative.transcript, result.final ? "Final" : "Interim", alternative.confidence);
                Debug.Log(text);
            }

            if (result.keywords_result == null || result.keywords_result.keyword == null)
                continue;
            foreach(var keyword in result.keywords_result.keyword)
            {
                string text = string.Format("keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
                Debug.Log(text);
            }
        }
        if (results.HasFinalResult())
        {
            StopListening();
            OnRecognition.Invoke(results.results.Fin)
        }*/
    }
    private void OnRecognizeSpeaker(SpeakerRecognitionEvent results)
    {
        if (results == null)
            return;
        Debug.Log($"On Recognize Speaker:");
        foreach (SpeakerLabelsResult labelResult in results.speaker_labels)
        {
            Debug.Log($"speaker result: {labelResult.speaker} | confidence: {labelResult.confidence} | from: {labelResult.from} " +
                $"| to: {labelResult.to}");
        }
    }
    #endregion

    private void OnDisable()
    {
        StopListening();
    }
    private void OnEnable()
    {
        GetAuthentication();
        Runnable.Run(InitService());
    }

    bool start = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!start)
            {
                StartListening();
                start = true;
            }
            else
            {
                StopListening();
                start = false;
            }
        }
         
    }
}
