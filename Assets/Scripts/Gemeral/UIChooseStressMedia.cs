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
        Text[] texts = DisplayTextBeforeOptions();
        WaitUntil wait = new WaitUntil(() => textDisplayEnded);
        yield return wait;
        TextData[] numbers = CreateNUmbers();
        RawImage[] decorators = DisplayNumberDecorators(numbers);
        Text[] options = DisplayOptions(numbers);
        wait = new WaitUntil(() => textDisplayEnded && audioPlayEnded);
        yield return wait;
        ArduinoMnager.Instance?.StartListeningLight();
        UIContainer.Instance.PlayAudio(data.listeningAudio, false);
        SpeechToText.Instance.StartListening(OnResult, data.maxRecordingSeconds);
        endChoosing = false;
        wait = new WaitUntil(() => endChoosing);
        yield return wait;
        if (result != -1)
            StartCoroutine(HasResultsHandler(texts, options, decorators));
        else
            StartCoroutine(DosentHasResultHandler(texts, options, decorators));
    }

    private IEnumerator DosentHasResultHandler(Text[] texts, Text[] options, RawImage[] decorators)
    {
        StartCoroutine(DestroyUIText(texts, data.fadeOutSeconds, data.fadeOutCurve));
        WaitUntil wait = new WaitUntil(() => textDestroyEnded);
        yield return wait;
        Text[] newTexts = CreateUITextsArray(data.unrecognizedText);
        StartCoroutine(PlayAudio(data.unrecognizedAudio, 0));
        StartCoroutine(DisplayUITexts(newTexts, data.unrecognizedText, data.fadeInSeconds, data.fadeInCurve, 0));
        wait = new WaitUntil(() => textDisplayEnded && audioPlayEnded);
        yield return wait;
        result = UnityEngine.Random.Range(3, 8);
        WaitForSeconds waitSeconds = new WaitForSeconds(1);
        yield return waitSeconds;
        StartCoroutine(DisplayResult(options, decorators));
        wait = new WaitUntil(() => resultHasDisplayed);
        yield return wait;
        waitSeconds = new WaitForSeconds(data.secondsAfterSelection);
        yield return waitSeconds;
        Text[] allTexts = new Text[newTexts.Length + 10];
        Array.Copy(newTexts, allTexts, newTexts.Length);
        Array.Copy(options, 0, allTexts, newTexts.Length, 10);
        StartCoroutine(DestroyUIText(allTexts, data.fadeOutSeconds, data.fadeOutCurve));
        StartCoroutine(DestroyDecorators(decorators));
        wait = new WaitUntil(() => textDestroyEnded);
        yield return wait;
        SceneManager.LoadScene(data.options2To10SceneName);
        isPlaying = false;
    }

    public IEnumerator HasResultsHandler(Text[] texts, Text[] options, RawImage[] decorators)
    {
        StartCoroutine(DisplayResult(options, decorators));
        WaitUntil wait = new WaitUntil(() => resultHasDisplayed);
        yield return wait;
        WaitForSeconds waitSeconds = new WaitForSeconds(data.secondsAfterSelection);
        yield return waitSeconds;
        Text[] allTexts = new Text[texts.Length + 10];
        Array.Copy(texts, allTexts, texts.Length);
        Array.Copy(options, 0, allTexts, texts.Length, 10);
        StartCoroutine(DestroyUIText(allTexts, data.fadeInSeconds, data.fadeOutCurve));
        StartCoroutine(DestroyDecorators(decorators));
        wait = new WaitUntil(() => textDestroyEnded);
        yield return wait;
        SceneManager.LoadScene((result == 1) ? data.option1SceneName : data.options2To10SceneName);
        isPlaying = false;
    }

    private Text[] DisplayTextBeforeOptions()
    {
        Text[] texts = CreateUITextsArray(data.textsBeforeOptionsDisplayed);
        StartCoroutine(PlayAudio(data.audio, data.secondsBeforePlayingAudio));
        StartCoroutine(DisplayUITexts(texts, data.textsBeforeOptionsDisplayed, data.fadeInSeconds, data.fadeInCurve, data.secondsBeforeDisplayingText));
        return texts;
    }
    private TextData[] CreateNUmbers()
    {
        TextData[] numbers = new TextData[10];
        for (int i = 0; i < 10; i++)
        {
            numbers[i] = new TextData()
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
        return numbers;
    }
    private Text[] DisplayOptions(TextData[] numbers)
    {
        
        Text[] options = new Text[10];
        float width = UIContainer.Instance.mainCanvas.pixelRect.width;
        float height = UIContainer.Instance.mainCanvas.pixelRect.height;
        for (int i = 0; i < 10; i++)
        {
            options[i] = CreateUIText(numbers[i], width, height);
        }
        StartCoroutine(DisplayUITexts(options, numbers, data.fadeInSeconds, data.fadeOutCurve, 0));
        return options;
    }

    private RawImage[] DisplayNumberDecorators(TextData[] numbers)
    {
        RawImage[] decorators = new RawImage[numbers.Length];
        float width = UIContainer.Instance.mainCanvas.pixelRect.width;
        float height = UIContainer.Instance.mainCanvas.pixelRect.height;
        for (int i = 0; i < numbers.Length; i++)
        {
            GameObject obj = new GameObject($"number {i} decorator");
            obj.transform.SetParent(UIContainer.Instance.mainCanvas.transform);
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
        ArduinoMnager.Instance?.StopListeningLight();
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
        resultHasDisplayed = false;
        float seconds = 0;
        if (result != 0)
            seconds = 0.6f;
        texts[result-1].color = data.selectedColor;
        decorators[result-1].color = data.selectedColor;
        UIContainer.Instance.PlayAudio(data.selectionAudio, false);
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
