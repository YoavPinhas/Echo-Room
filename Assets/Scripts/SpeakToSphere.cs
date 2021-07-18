using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpeakToSphere : MonoBehaviour
{
    
    private AudioClip recording;
    public TalkingSphereFromMicrophone[] talkingSpheres;
    public string NextSceneName;
    public void StartTalking()
    {
        recording = Microphone.Start(null, true, 120, 44000);
        StartCoroutine(VisualizeAudio());
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartNextScene());
        }
    }

    private IEnumerator StartNextScene()
    {
        WaitUntil wait = new WaitUntil(() => talkingSpheres[0].IsFadeOut && talkingSpheres[1].IsFadeOut && talkingSpheres[2].IsFadeOut);
        WaitForSeconds delay = new WaitForSeconds(2);
        foreach (TalkingSphereFromMicrophone sphere in talkingSpheres)
        {
            sphere.FadeOut();
        }
        yield return wait;
        yield return delay;
        SceneManager.LoadScene(NextSceneName);
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
        StartCoroutine(Talk());
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
            Debug.Log($"t = {t}");
            foreach(TalkingSphereFromMicrophone sphere in talkingSpheres)
                sphere.SetHeight(t);
            yield return waitFrame;
        }
    }

    private float GetRMS()
    {
        return GetRMSFromChannel(0);// + GetRMSFromChannel(1);
    }

    private float GetRMSFromChannel(int channel)
    {
        if (Microphone.GetPosition(null) <= 4096)
            return 0;
        float[] samples = new float[4096];
        recording.GetData(samples, Microphone.GetPosition(null) - 4096);
        float sum = 0;
        foreach (float f in samples)
        {
            sum += f * f;
        }
        return Mathf.Sqrt(sum);
    }

    
}
