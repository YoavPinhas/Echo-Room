using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;
using UnityEngine.SceneManagement;

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
        wait = new WaitUntil(() => textDisplayEnded && audioPlayEnded);
        yield return wait;
        hasResults = false;
        ArduinoMnager.Instance.StartListeningLight();
        UIContainer.Instance.PlayAudio(data.listeningAudio, false);
        SpeechToText.Instance.StartListening(OnResults);
        WaitUntil waitForResults = new WaitUntil(() => hasResults);
        yield return waitForResults;
        if (resultIndex == -1)
            resultIndex = options.Length-1;
        
        if(resultIndex < options.Length)
            DisplayResult(options[resultIndex]);
        StartCoroutine(DestroyUIText(texts, data.fadeInSeconds, data.fadeInCurve));
        WaitForSeconds waitForSeconds = new WaitForSeconds(data.secondsBeforeFadeOut);
        yield return waitForSeconds;
        StartCoroutine(DestroyUIText(options, data.fadeInSeconds, data.fadeInCurve));
        wait = new WaitUntil(() => textDestroyEnded);
        yield return wait;
        SceneManager.LoadScene(data.evaluator.Options[resultIndex].optionName);
        
        isPlaying = false;
    }

    private void DisplayResult(Text text)
    {
        text.color = data.SelectionColor;
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
        ArduinoMnager.Instance.StopListeningLight();
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
}
