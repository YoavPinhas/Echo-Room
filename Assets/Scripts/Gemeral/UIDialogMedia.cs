using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

public class UIDialogMedia : UIMedia
{
    private DialogData data;
    public override IEnumerator Play()
    {
        isPlaying = true;
        StartCoroutine(PlayAudio(data.audio, data.secondsBeforePlayingAudio));
        
        Text[] texts = CreateUITextsArray(data.texts);
        if(texts != null && texts.Length > 0)
            StartCoroutine(DisplayUITexts(texts, data.texts, data.fadeInSeconds, data.fadeInCurve, data.secondsBeforeDisplayingText));
       
        WaitUntil wait = new WaitUntil(() => textDisplayEnded && audioPlayEnded);
        yield return wait;

        if (texts != null && texts.Length > 0)
            StartCoroutine(DestroyUIText(texts, data.fadeOutSeconds, data.fadeOutCurve));
        
        wait = new WaitUntil(() => textDestroyEnded);
        yield return wait;
        isPlaying = false;
    }

    public override void SetData(Data data)
    {
        this.data = (DialogData)data;
    }
}
