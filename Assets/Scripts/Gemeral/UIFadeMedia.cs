using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFadeMedia : UIMedia
{
    private FadeData data;
    public override IEnumerator Play()
    {
        if(data.fadeIn)
        {
            isPlaying = true;
            WaitWhile wait = new WaitWhile(() => TalkingSphere.Instance.IsFadeIn);
            Debug.Log(TalkingSphere.Instance + "Fade In");
            TalkingSphere.Instance?.FadeIn();
            yield return wait;
            isPlaying = false;
        }
        else
        {
            isPlaying = false;
            Debug.Log(TalkingSphere.Instance + "Fade Out");
            TalkingSphere.Instance?.FadeOut();
        }
    }

    public override void SetData(Data data)
    {
        this.data = (FadeData)data;
    }
}
