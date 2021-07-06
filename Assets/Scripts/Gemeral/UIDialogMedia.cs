using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

public class UIDialogMedia : UIMedia
{
    public override IEnumerator Play()
    {
        DialogData _data = (DialogData)data;
        isPlaying = true;

        if (((DialogData)data).audio != null)
            StartCoroutine(PlayAudio(_data.audio, _data.secondsBeforePlayingAudio));
        else
            audioPlayEnded = true;
        Text[] texts = CreateUITextsArray(_data.texts);
        StartCoroutine(DisplayUITexts(texts, _data.texts, _data.fadeInSeconds, _data.fadeInCurve, _data.secondsBeforeDisplayingText));
        //var waitForDisplay = new WaitUntil(() => textDisplayEnded && audioPlayEnded);
        //yield return waitForDisplay;
        yield return new WaitUntil(() => textDisplayEnded && audioPlayEnded);
        StartCoroutine(DestroyUIText(texts, _data.fadeOutSeconds, _data.fadeOutCurve));
        //var waitForDestroy = new WaitUntil(() => textDestroyEnded);
        //yield return waitForDestroy;
        yield return new WaitUntil(() => textDestroyEnded);
        isPlaying = false;
    }
}
