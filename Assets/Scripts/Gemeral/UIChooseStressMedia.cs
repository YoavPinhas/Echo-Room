using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

public class UIChooseStressMedia : UIMedia
{
    public override IEnumerator Play()
    {
        ChooseStressLevelData _data = (ChooseStressLevelData)data;
        isPlaying = true;
        int length = _data.textsBeforeOptionsDisplayed.Length;
        TextData[] texts = new TextData[length + 10];
        Array.Copy(_data.textsBeforeOptionsDisplayed, texts, length);
        for (int i = 0; i < 10; i++)
        {
            texts[length + i] = new TextData()
            {
                text = $"{i + 1}",
                color = _data.numbersFormat.color,
                duration = (i == 9) ? _data.numbersFormat.duration : 0,
                font = _data.numbersFormat.font,
                fontSize = _data.numbersFormat.fontSize,
                sceenX = 0.1f + i * (0.8f / 9f),
                sceenY = _data.numbersFormat.sceenY
            };
        }
        Text[] uiTexts = CreateUITextsArray(texts);
        StartCoroutine(PlayAudio(_data.audio, _data.secondsBeforePlayingAudio));
        StartCoroutine(DisplayUITexts(uiTexts, texts, _data.fadeInSeconds, _data.fadeInCurve, _data.secondsBeforeDisplayingText));
        var waitForDisplay = new WaitUntil(() => textDisplayEnded && audioPlayEnded);
        yield return waitForDisplay;
        SpeechToText.Instance.StartListening((str) => StartCoroutine(OnResult(_data, str, uiTexts)));
    }

    private IEnumerator OnResult(ChooseStressLevelData data, string text, Text[] texts)
    {
        string res = data.evaluator.Choose(text);
        Debug.Log(res);
        StartCoroutine(DestroyUIText(texts, data.fadeOutSeconds, data.fadeOutCurve));
        var waitForDestroy = new WaitUntil(() => textDestroyEnded);
        yield return waitForDestroy;
        TextData t = new TextData { text = res };
        Text[] uiT = CreateUITextsArray(new TextData[] { t });
        isPlaying = false;
    }

}
