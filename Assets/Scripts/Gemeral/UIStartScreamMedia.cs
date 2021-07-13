using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStartScreamMedia : UIMedia
{
    StartScreamData data;
    public override IEnumerator Play()
    {
        isPlaying = true;
        MicrophonLevel.Instance.StartMicrophone();
        yield return new WaitForSeconds(data.maxSeconds);
        MicrophonLevel.Instance.StopMicrophone();
        isPlaying = false;
    }

    public override void SetData(Data data)
    {
        this.data = (StartScreamData)data;
    }
}
