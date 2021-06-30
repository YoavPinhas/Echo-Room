using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIPlayer : MonoBehaviour
{
    [SerializeField] private Data[] dataToPlay;
    [SerializeField] private Canvas canvas;
    [SerializeField] private AudioSource audioSource;

    private bool isPlaying = false;
    private bool thereIsText = false;
    private bool thereIsAudio = false;
    private int currentDataIndex = -1;
    private WaitWhile waitIfAudioIsPlaying;
    void Start()
    {
        waitIfAudioIsPlaying = new WaitWhile(() => audioSource.isPlaying);
    }


    // Update is called once per frame
    void Update()
    {
        if (!isPlaying)
        {
            currentDataIndex++;
            if (currentDataIndex >= dataToPlay.Length)
                return;
            switch (dataToPlay[currentDataIndex].GetDataType())
            {
                case DataType.DialogData:
                    StartCoroutine(PlayDialogData((DialogData)dataToPlay[currentDataIndex]));
                    return;
                case DataType.ChooseFrustrationLevel:
                    StartCoroutine(PlayChooseFrustrationLevelData((ChooseFrustrationLevelData)dataToPlay[currentDataIndex]));
                    return;
            }
        }
    }

    IEnumerator PlayText(TextData[] textsData, float secondsBeforeDisplaying,
        float fadeInSeconds, AnimationCurve fadeInCurve, 
        float fadeOutSeconds, AnimationCurve fadeOutCurve)
    {
        thereIsText = true;
        yield return new WaitForSeconds(secondsBeforeDisplaying);
        int textIndex = 0;
        List<GameObject> texts = new List<GameObject>();
        var width = canvas.pixelRect.width;
        var height = canvas.pixelRect.height;

        while (textIndex < textsData.Length)
        {
            texts.Add(CreateDialogText(textsData[textIndex], fadeInSeconds, fadeInCurve, width, height, $"text {textIndex}"));
            yield return new WaitForSeconds(textsData[textIndex].duration);
            textIndex++;
        }
        foreach (GameObject o in texts)
        {
            o.GetComponent<FadeText>().FadeOut(fadeOutSeconds, fadeOutCurve);
        }
        yield return new WaitForSeconds(fadeOutSeconds);
        foreach (GameObject o in texts)
        {
            Destroy(o);
        }
        texts = null;
        thereIsText = false;
    }
    IEnumerator PlayAudio(AudioClip audio, float secondsBeforeAudio)
    {
        thereIsAudio = true;
        yield return new WaitForSeconds(secondsBeforeAudio);
        audioSource.PlayOneShot(audio);
        yield return waitIfAudioIsPlaying;
        thereIsAudio = false;
    }


    IEnumerator PlayChooseFrustrationLevelData(ChooseFrustrationLevelData frustrationLevel)
    {
        PlayChooseFrustrationLevelText(frustrationLevel);
        yield return null;
    }

    private void PlayChooseFrustrationLevelText(ChooseFrustrationLevelData data)
    {
        int length1 = data.textsBeforeOptionsDisplayed.Length;
        Type fsType = typeof(ChooseFrustrationLevelData);
        TextData[] texts = new TextData[length1 + 10];
        Array.Copy(data.textsBeforeOptionsDisplayed, texts, length1);
        texts[length1] = data.levelIs1;
        texts[length1 + 1] = data.levelIs2;
        texts[length1 + 2] = data.levelIs3;
        texts[length1 + 3] = data.levelIs4;
        texts[length1 + 4] = data.levelIs5;
        texts[length1 + 5] = data.levelIs6;
        texts[length1 + 6] = data.levelIs7;
        texts[length1 + 7] = data.levelIs8;
        texts[length1 + 8] = data.levelIs9;
        texts[length1 + 9] = data.levelIs10;
        StartCoroutine(PlayText(texts, data.secondsBeforeDisplayingText, data.fadeInSeconds, data.fadeInCurve
                                , data.fadeOutSeconds, data.fadeOutCurve));
    }

    #region DialogData Methods
    IEnumerator PlayDialogData(DialogData dialog)
    {
        isPlaying = true;
        if (dialog.audio != null)
            PlayDialogAudio(dialog);
        PlayDialogText(dialog);
        yield return new WaitUntil(() => thereIsText == false && thereIsAudio == false);
        isPlaying = false;
    }
    
    private void PlayDialogText(DialogData dialog)
    {
        StartCoroutine(PlayText(dialog.texts, dialog.secondsBeforeDisplayingText, dialog.fadeInSeconds,
                                dialog.fadeInCurve, dialog.fadeOutSeconds, dialog.fadeOutCurve));
    }
    private void PlayDialogAudio(DialogData dialog)
    {
        StartCoroutine(PlayAudio(dialog.audio, dialog.secondsBeforePlayingAudio));
    }
    private GameObject CreateDialogText(TextData data, float fadeInSeconds, AnimationCurve curve, float width, float height, string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(canvas.transform);
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();

        text.alpha = 0;
        var posX = data.sceenX * width - 0.5f * width;
        var posY = data.sceenY * height - 0.5f * height;
        text.rectTransform.anchoredPosition = new Vector3(posX, posY, 0);
        text.text = data.text;
        if(data.font != null)
        {
            text.font = data.font;
        }
        text.color = data.color;
        text.autoSizeTextContainer = true;
        text.fontSize = data.fontSize;
        FadeText fade = text.gameObject.AddComponent<FadeText>();
        fade.text = text;
        fade.FadeIn(fadeInSeconds, curve);
        return obj;
    }
    
    #endregion
}
