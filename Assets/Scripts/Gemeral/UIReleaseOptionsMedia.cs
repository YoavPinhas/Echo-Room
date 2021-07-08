using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

public class UIReleaseOptionsMedia : UIMedia
{
    private ReleaseOptionsData data;
    public override IEnumerator Play()
    {
        
        StartCoroutine(PlayAudio(data.audio, data.secondsBeforePlayingAudio));

        Text[] texts = CreateUITextsArray(data.texts);

        StartCoroutine(DisplayUITexts(texts, data.texts, data.fadeInSeconds, data.fadeInCurve, data.secondsBeforeDisplayingText));

        WaitUntil wait = new WaitUntil(() => textDisplayEnded && audioPlayEnded);
        yield return wait;

        
    }

    public override void SetData(Data data)
    {
        this.data = (ReleaseOptionsData)data;
    }
}
