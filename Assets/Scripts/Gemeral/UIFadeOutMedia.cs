using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFadeOutMedia : UIMedia
{
    private FadeOutData data;
    public override IEnumerator Play()
    {
        isPlaying = true;
        yield return null;
        UIContainer.Instance.FadeOut(data.fadeSeconds, data.fadeCurve);
        yield return new WaitUntil(() => UIContainer.Instance.IsFadeOut);
        isPlaying = false;
    }

    public override void SetData(Data data)
    {
        this.data = (FadeOutData)data;
    }
}
