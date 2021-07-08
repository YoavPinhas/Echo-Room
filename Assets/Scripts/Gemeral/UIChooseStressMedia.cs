using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Text = TMPro.TextMeshProUGUI;

public class UIChooseStressMedia : UIMedia
{
    private ChooseStressLevelData data;
    private bool endChoosing;
    private bool resultHasDisplayed;
    private int result;
    public  override IEnumerator Play()
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
                sceenX = 0.1f + i * (0.8f / 9f),
                sceenY = data.numbersFormat.sceenY
            };
        }
        Text[] uiTexts = CreateUITextsArray(texts);
        StartCoroutine(PlayAudio(data.audio, data.secondsBeforePlayingAudio));
        
        StartCoroutine(DisplayUITexts(uiTexts, texts, data.fadeInSeconds, data.fadeInCurve, data.secondsBeforeDisplayingText));

        WaitUntil wait = new WaitUntil(() => textDisplayEnded && audioPlayEnded);
        yield return wait;

        WaitForSeconds delay = new WaitForSeconds(1);

        yield return delay;

        SpeechToText.Instance.StartListening((str) => StartCoroutine(OnResult(str)), data.maxRecordingSeconds);

        wait = new WaitUntil(() => endChoosing);
        yield return wait;

        resultHasDisplayed = false;
        StartCoroutine(DisplayResult(uiTexts.Skip(length).Take(10).ToArray()));

        wait = new WaitUntil(() => resultHasDisplayed);
        yield return wait;

        WaitForSeconds waitBeforeDestroy = new WaitForSeconds(data.numbersFormat.duration);
        yield return waitBeforeDestroy;

        StartCoroutine(DestroyUIText(uiTexts, data.fadeOutSeconds, data.fadeOutCurve));
        WaitUntil waitForDestroy = new WaitUntil(() => textDestroyEnded);
        yield return waitForDestroy;
        if(result == 1)
            SceneManager.LoadScene(data.option1SceneName);
        else
            SceneManager.LoadScene(data.options2To10SceneName);
        isPlaying = false;
    }

    public override void SetData(Data data)
    {
        this.data = (ChooseStressLevelData)data;
    }

    private IEnumerator OnResult(string text)
    {
        string res = data.evaluator.Choose(text);
        Debug.Log(res);
        result = int.Parse(res);
        yield return null;
        endChoosing = true;
    }

    private IEnumerator DisplayResult(Text[] texts)
    {
        float seconds = 0;
        if (result != 0)
            seconds = 0.6f;
        List<GameObject> objects = new List<GameObject>();
        Vector3 offset = new Vector3(0, 0, 1f);
        Vector3 direction = (texts[0].rectTransform.position - Camera.main.transform.position).normalized;
        Vector3 startPosition = texts[0].rectTransform.position + 2* direction;
        
        for(int i = 0; i < result; i++)
        {
            //Debug.Log(texts[i].rectTransform.position);
            direction = (texts[i].rectTransform.position - Camera.main.transform.position).normalized;
            Vector3 finalePosition = texts[i].rectTransform.position + 2*direction;
            objects.Add(Instantiate(data.displayResultPrefab, startPosition, Quaternion.identity));
            Tween tween = objects[i].AddComponent<Tween>();
            tween.StartTween(startPosition, finalePosition, seconds, 1);
            startPosition = finalePosition;
            yield return tween.TweenMotionCompleted;
        }
        resultHasDisplayed = true;
    }

}
