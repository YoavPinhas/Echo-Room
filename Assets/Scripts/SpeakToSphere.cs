using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakToSphere : MonoBehaviour
{
    
    private AudioClip recording;
    public TalkingSphere talkingSphere;
    public void StartTalking()
    {
        recording = Microphone.Start(null, false, 120, 44000);
        StartCoroutine(VisualizeAudio());
    }
    private void Update()
    {
      
    }
    public void StopTalking()
    {
        Microphone.End(null);
    }

    private void OnDisable()
    {
        StopTalking();
    }

    private void Start()
    {
        if (!Microphone.IsRecording(null))
        {
            StartCoroutine(Talk());
        }
    }

    private void OnEnable()
    {
        if (!Microphone.IsRecording(null))
        {
            StartCoroutine(Talk());
        }
    }

    private IEnumerator Talk()
    {
        yield return new WaitForSeconds(1);
        StartTalking();
    }

    private IEnumerator VisualizeAudio()
    {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        float multiplyer = 50f;
        float min = 0f;
        float max = 10f;
        while (Microphone.IsRecording(null))
        {
            //float rms = GetRMS();
            float t = Mathf.InverseLerp(min, max, GetRMS());
            talkingSphere.SetHeight(t);
            yield return waitFrame;
        }
    }

    private float GetRMS()
    {
        return GetRMSFromChannel(0) + GetRMSFromChannel(1);
    }

    private float GetRMSFromChannel(int channel)
    {
        if (recording.length <= 4096)
            return 0;
        float[] samples = new float[4096];
        recording.GetData(samples, channel);
        float sum = 0;
        foreach (float f in samples)
        {
            sum += f * f;
        }
        return Mathf.Sqrt(sum);
    }
}
