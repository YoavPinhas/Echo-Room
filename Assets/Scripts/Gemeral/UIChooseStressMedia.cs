using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Text = TMPro.TextMeshProUGUI;
using UnityEngine.UI;

public class UIChooseStressMedia : UIMedia
{
    private ChooseStressLevelData data;
    private bool endChoosing;
    private bool resultHasDisplayed;
    private int result;



    public override IEnumerator Play()
    {
        isPlaying = true;
        endChoosing = false;
        int length = data.textsBeforeOptionsDisplayed.Length;
        TextData[] texts = new TextData[length + 10];
        Array.Copy(data.textsBeforeOptionsDisplayed, texts, length);
        float finaleDuration = data.numbersFormat.duration;
        for (int i = 0; i < 10; i++)
        {
            texts[length + i] = new TextData()
            {
                text = $"{i + 1}",
                color = data.numbersFormat.color,
                duration = 0,
                font = data.numbersFormat.font,
                fontSize = data.numbersFormat.fontSize,
                sceenX = 0.2f + i * (0.6f / 9f),
                sceenY = data.numbersFormat.sceenY
            };
        }
        Text[] uiTexts = CreateUITextsArray(texts);
        StartCoroutine(PlayAudio(data.audio, data.secondsBeforePlayingAudio));
        StartCoroutine(DisplayUITexts(uiTexts, texts, data.fadeInSeconds, data.fadeInCurve, data.secondsBeforeDisplayingText));
        float duration = 0;
        for (int i = 0; i < length; i++)
        {
            duration += texts[i].duration;
        }
        WaitForSeconds delay = new WaitForSeconds(duration);
        yield return delay;

        RawImage[] decorators = DisplayNumberDecorators(texts.Skip(length).Take(10).ToArray());
        WaitUntil wait = new WaitUntil(() => textDisplayEnded && audioPlayEnded);
        yield return wait;

        delay = new WaitForSeconds(1);

        yield return delay;

        SpeechToText.Instance.StartListening((str) => OnResult(str), data.maxRecordingSeconds);

        wait = new WaitUntil(() => endChoosing);
        yield return wait;

        if (result != -1)
        {
            resultHasDisplayed = false;
            StartCoroutine(DisplayResult(uiTexts.Skip(length).Take(10).ToArray(), decorators));

            wait = new WaitUntil(() => resultHasDisplayed);
            yield return wait;

            WaitForSeconds waitBeforeDestroy = new WaitForSeconds(data.numbersFormat.duration);
            yield return waitBeforeDestroy;

            StartCoroutine(DestroyUIText(uiTexts, data.fadeOutSeconds, data.fadeOutCurve));
            StartCoroutine(DestroyDecorators(decorators));
            WaitUntil waitForDestroy = new WaitUntil(() => textDestroyEnded);
            yield return waitForDestroy;
            if (result == 1)
                SceneManager.LoadScene(data.option1SceneName);
            else
                SceneManager.LoadScene(data.options2To10SceneName);
        }
        else
        {

        }
        isPlaying = false;
    }

    private RawImage[] DisplayNumberDecorators(TextData[] numbers)
    {
        RawImage[] decorators = new RawImage[numbers.Length];
        float width = canvas.pixelRect.width;
        float height = canvas.pixelRect.height;
        for (int i = 0; i < numbers.Length; i++)
        {
            GameObject obj = new GameObject($"number {i} decorator");
            obj.transform.SetParent(canvas.transform);
            decorators[i] = obj.AddComponent<RawImage>();
            var posX = (numbers[i].sceenX - 0.5f) * width;
            var posY = (numbers[i].sceenY - 0.45f) * height;
            decorators[i].rectTransform.anchoredPosition = new Vector3(posX, posY, 0);
            decorators[i].texture = data.numbersDecorator;
            decorators[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 15);
            decorators[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 15);
            Fade fade = obj.AddComponent<Fade>();
            fade.text = decorators[i];
            fade.FadeIn(data.fadeInSeconds, data.fadeInCurve);
        }
        return decorators;
    }

    public override void SetData(Data data)
    {
        this.data = (ChooseStressLevelData)data;
    }

    private void OnResult(string text)
    {
        string res = data.evaluator.Choose(text);
        if (res != null)
        {
            Debug.Log(res);
            result = int.Parse(res);
        }
        else
        {
            result = -1;
        }
        endChoosing = true;
    }

    private IEnumerator DisplayResult(Text[] texts, RawImage[] decorators)
    {
        float seconds = 0;
        if (result != 0)
            seconds = 0.6f;
        texts[result-1].color = data.selectedColor;
        decorators[result-1].color = data.selectedColor;
        WaitForSeconds wait = new WaitForSeconds(seconds);
        yield return wait;
        resultHasDisplayed = true;
    }

    private IEnumerator DestroyDecorators(RawImage[] decorators)
    {
        foreach (RawImage decorator in decorators)
        {
            decorator.gameObject.GetComponent<Fade>().FadeOut(data.fadeOutSeconds, data.fadeOutCurve);
        }
        yield return new WaitForSeconds(data.fadeOutSeconds);
        foreach (RawImage decorator in decorators)
        {
            Destroy(decorator.gameObject);
        }
    }

}
