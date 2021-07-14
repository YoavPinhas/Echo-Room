using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFadeMedia : UIMedia
{
    private FadeData data;
    public override IEnumerator Play()
    {
        isPlaying = true;
        if(data.fadeIn)
        {
            WaitWhile wait = new WaitWhile(() => TalkingSphere.Instance.IsFadeIn);
            Debug.Log(TalkingSphere.Instance + "Fade In");
            TalkingSphere.Instance?.FadeIn();
            yield return wait;
        }
        else
        {
            WaitWhile wait = new WaitWhile(() => TalkingSphere.Instance.IsFadeOut);
            Debug.Log(TalkingSphere.Instance + "Fade Out");
            TalkingSphere.Instance?.FadeOut();
            yield return wait;
        }
        isPlaying = false;
    }

    public override void SetData(Data data)
    {
        this.data = (FadeData)data;
    }
}
