using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISelectionMedia : UIMedia
{
    private SelectData data;
    private bool hasResults = false;
    private bool canGoToNextPhase = false;
    private int resultIndex;
    public override IEnumerator Play()
    {
        isPlaying = true;
        Text[] texts = DisplayTextBeoreSelection();
        WaitUntil wait = new WaitUntil(() => textDisplayEnded);
        yield return wait;
        Text[] options = DisplayOptions();
        RawImage[] decorators = DisplayOptionsDecorators(data.selectionOptions);
        wait = new WaitUntil(() => textDisplayEnded && audioPlayEnded);
        yield return wait;
        hasResults = false;
        ArduinoMnager.Instance?.StartListeningLight();
        UIContainer.Instance.PlayAudio(data.listeningAudio, false);
        SpeechToText.Instance.StartListening(OnResults, data.maxRecordingSeconds);
        WaitUntil waitForResults = new WaitUntil(() => hasResults);
        yield return waitForResults;
        if (resultIndex == -1)
            resultIndex = data.evaluator.Options.Length-1;
        
        if(resultIndex < options.Length)
            DisplayResult(options[resultIndex], decorators[resultIndex]);
        StartCoroutine(DestroyUIText(texts, data.fadeInSeconds, data.fadeInCurve));
        WaitForSeconds waitForSeconds = new WaitForSeconds(data.secondsBeforeFadeOut);
        yield return waitForSeconds;
        StartCoroutine(DestroyDecorators(decorators));
        StartCoroutine(DestroyUIText(options, data.fadeInSeconds, data.fadeInCurve));
        wait = new WaitUntil(() => textDestroyEnded);
        yield return wait;
        SceneManager.LoadScene(data.evaluator.Options[resultIndex].optionName);
        
        isPlaying = false;
    }

    private void DisplayResult(Text text, RawImage decorator)
    {
        text.color = data.SelectionColor;
        decorator.color = data.SelectionColor;
        UIContainer.Instance.PlayAudio(data.selectionAudio, false);
    }

    private Text[] DisplayOptions()
    {
        Text[] options = CreateUITextsArray(data.selectionOptions);
        StartCoroutine(DisplayUITexts(options, data.selectionOptions, data.fadeInSeconds, data.fadeOutCurve, 0));
        return options;
    }

    private Text[] DisplayTextBeoreSelection()
    {
        Text[] texts = CreateUITextsArray(data.textsBeforeSelection);
        StartCoroutine(PlayAudio(data.audio, data.secondsBeforePlayingAudio));
        StartCoroutine(DisplayUITexts(texts, data.textsBeforeSelection, data.fadeInSeconds, data.fadeInCurve, data.secondsBeforeDisplayingText));
        return texts;
    }

    private void OnResults(string res)
    {
        ArduinoMnager.Instance?.StopListeningLight();
        string result = data.evaluator.Choose(res);
        resultIndex = -1;
        for (int i = 0; i < data.evaluator.Options.Length; i++)
        {
            if(result == data.evaluator.Options[i].optionName)
            {
                resultIndex = i;
                break;
            }
        }
        hasResults = true;
    }

    public override void SetData(Data data)
    {
        this.data = (SelectData)data;
    }


    private RawImage[] DisplayOptionsDecorators(TextData[] options)
    {
        RawImage[] decorators = new RawImage[options.Length];
        float width = UIContainer.Instance.mainCanvas.pixelRect.width;
        float height = UIContainer.Instance.mainCanvas.pixelRect.height;
        for (int i = 0; i < options.Length; i++)
        {
            GameObject obj = new GameObject($"number {i} decorator");
            obj.transform.SetParent(UIContainer.Instance.mainCanvas.transform);
            decorators[i] = obj.AddComponent<RawImage>();
            var posX = (options[i].sceenX - 0.5f) * width;
            var posY = (options[i].sceenY - 0.45f) * height;
            decorators[i].rectTransform.anchoredPosition = new Vector3(posX, posY, 0);
            decorators[i].texture = data.optionDecorator;
            decorators[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 15);
            decorators[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 15);
            Fade fade = obj.AddComponent<Fade>();
            fade.text = decorators[i];
            fade.FadeIn(data.fadeInSeconds, data.fadeInCurve);
        }
        return decorators;
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
