using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakToSphere : MonoBehaviour
{
    public UIContainer container;
    private AudioClip recording;
    public void StartTalking()
    {
        container.PlayAudio(Microphone.Start(null, false, 120, 44000));
    }

    public void StopTalking()
    {
        Microphone.End(null);
    }

    private void OnDisable()
    {
        StopTalking();
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
        yield return new WaitForSeconds(3);
        StartTalking();
    }
}
