using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Text = TMPro.TextMeshProUGUI;
[RequireComponent(typeof(AudioSource))]
public class UIPlayer : MonoBehaviour
{
    [SerializeField] private Data[] dataToPlay;
    [SerializeField] private Canvas canvas;
    private AudioSource audioSource;

    private bool isPlaying = false;
    private bool thereIsText = false;
    private bool audioIsEnded = false;
    private bool textDisplayEnded = false;
    private bool textDestroyEnded = false;
    private int currentDataIndex = -1;
    private WaitWhile waitIfAudioIsPlaying;
    private Dictionary<Data, UIMedia> uiMedia = new Dictionary<Data, UIMedia>();
    UIMedia currentMedia = null;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        waitIfAudioIsPlaying = new WaitWhile(() => audioSource.isPlaying);
    }

    void Update()
    {
        if (currentMedia == null || !currentMedia.IsPlaying)
        {
            currentDataIndex++;
            if (currentDataIndex >= dataToPlay.Length)
                return;
            CreateMedia(dataToPlay[currentDataIndex]);
            StartCoroutine(currentMedia.Play());
        }
    }

    private void CreateMedia(Data data)
    {
        Destroy(currentMedia);
        switch (data.GetDataType())
        {
            case DataType.DialogData:
                currentMedia = gameObject.AddComponent<UIDialogMedia>();
                currentMedia.data = (DialogData)data;
                break;
            case DataType.StressLevel:
                currentMedia = gameObject.AddComponent<UIChooseStressMedia>();
                currentMedia.data = (ChooseStressLevelData)data;
                break;
            case DataType.ReleaseOption:
                break;
        }
        currentMedia.audioSource = audioSource;
        currentMedia.canvas = canvas;
    }

    //IEnumerator PlayAudio(AudioClip audio, float secondsBeforeAudio)
    //{
    //    audioIsEnded = false;
    //    yield return new WaitForSeconds(secondsBeforeAudio);
    //    audioSource.PlayOneShot(audio);
    //    yield return waitIfAudioIsPlaying;
    //    audioIsEnded = true;
    //}
    //IEnumerator PlayChooseStressLevelData(ChooseStressLevelData stressLevel)
    //{
    //    StartCoroutine(PlayChooseStressLevelText(stressLevel));
    //    yield return new WaitUntil(() => textDisplayEnded && audioIsEnded);
    //    SpeechToText.Instance.StartListening((str) => {
    //        GameObject obj = new GameObject(name);
    //        obj.transform.SetParent(canvas.transform);
    //        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
    //        text.text = stressLevel.evaluator.Choose(str);
    //        var width = canvas.pixelRect.width;
    //        var height = canvas.pixelRect.height;
    //        var posX = 0.5f * width - 0.5f * width;
    //        var posY = 0.2f * height - 0.5f * height;
    //        text.rectTransform.anchoredPosition = new Vector3(posX, posY, 0);
    //    });


    //}

    //private IEnumerator PlayChooseStressLevelText(ChooseStressLevelData data)
    //{
    //    isPlaying = true;
    //    int length1 = data.textsBeforeOptionsDisplayed.Length;
    //    Type fsType = typeof(ChooseStressLevelData);
    //    TextData[] texts = new TextData[length1 + 10];
    //    Array.Copy(data.textsBeforeOptionsDisplayed, texts, length1);
    //    for (int i = 0; i < 10; i++)
    //    {
    //        texts[length1 + i] = new TextData()
    //        {
    //            text = $"{i + 1}",
    //            color = data.numbersFormat.color,
    //            duration = (i == 9) ? data.numbersFormat.duration : 0,
    //            font = data.numbersFormat.font,
    //            fontSize = data.numbersFormat.fontSize,
    //            sceenX = 0.1f + i * (0.8f / 9f),
    //            sceenY = data.numbersFormat.sceenY
    //        };
    //    }
    //    Text[] uiTexts = CreateUITextsArray(texts, canvas);
    //    StartCoroutine(PlayAudio(data.audio, data.secondsBeforePlayingAudio));
    //    StartCoroutine(DisplayUITexts(uiTexts, texts, data.fadeInSeconds, data.fadeInCurve, data.secondsBeforeDisplayingText));
    //    yield return new WaitUntil(() => textDisplayEnded && audioIsEnded);
    //    isPlaying = false;
    //}

    //#region DialogData Methods
    //IEnumerator PlayDialogData(DialogData dialog)
    //{
    //    isPlaying = true;
    //    if (dialog.audio != null)
    //        PlayDialogAudio(dialog);
    //    StartCoroutine(PlayDialogText(dialog));
    //    yield return new WaitUntil(() => textDestroyEnded && audioIsEnded);
    //    isPlaying = false;
    //}

    //private IEnumerator PlayDialogText(DialogData dialog)
    //{
    //    Text[] texts = CreateUITextsArray(dialog.texts, canvas);
    //    StartCoroutine(DisplayUITexts(texts, dialog.texts, dialog.fadeInSeconds, dialog.fadeInCurve, dialog.secondsBeforeDisplayingText));
    //    yield return new WaitUntil(() => textDisplayEnded);
    //    StartCoroutine(DistroyUIText(texts, dialog.fadeOutSeconds, dialog.fadeOutCurve));
    //    yield return new WaitUntil(() => textDestroyEnded);
    //}
    //private void PlayDialogAudio(DialogData dialog)
    //{
    //    StartCoroutine(PlayAudio(dialog.audio, dialog.secondsBeforePlayingAudio));
    //}

    //#endregion

    //private Text[] CreateUITextsArray(TextData[] inputTexts, Canvas canvas)
    //{
    //    if(inputTexts == null || inputTexts.Length == 0)
    //    {
    //        Debug.LogError("Create UI Text Array need to get valid non empty array.", this);
    //        return null;
    //    }
    //    var width = canvas.pixelRect.width;
    //    var height = canvas.pixelRect.height;
    //    Text[] result = new Text[inputTexts.Length];
    //    for (int i = 0; i < inputTexts.Length; i++)
    //    {
    //        result[i] = CreateUIText(inputTexts[i], width, height, canvas, $"Text {i}");
    //    }
    //    return result;
    //}
    //private Text CreateUIText(TextData inputText, float width, float height, Canvas canvas, string name = "")
    //{
    //    GameObject obj = new GameObject(name);
    //    obj.transform.SetParent(canvas.transform);
    //    Text text = obj.AddComponent<Text>();
    //    var posX = (inputText.sceenX - 0.5f) * width;
    //    var posY = (inputText.sceenY - 0.5f) * width;
    //    text.rectTransform.anchoredPosition = new Vector3(posX, posY, 0);
    //    FadeText fade = obj.AddComponent<FadeText>();
    //    fade.text = text;
    //    text.text = inputText.text;
    //    text.alpha = 0;
    //    return text;
    //}

    //private IEnumerator DisplayUITexts(Text[] texts, TextData[] data, float fadeInSeconds, AnimationCurve fadeInCurve, float delay)
    //{
    //    if(texts == null || data == null || texts.Length == 0 || data.Length == 0 || texts.Length != data.Length)
    //    {
    //        Debug.LogError("Texts and data must be a valid arrays with same length for DisplayUIText to work.", this);
    //        yield break;
    //    }
    //    yield return new WaitForSeconds(delay);
    //    textDisplayEnded = false;
    //    for (int i = 0; i < texts.Length; i++)
    //    {
    //        texts[i].GetComponent<FadeText>().FadeIn(fadeInSeconds, fadeInCurve);
    //        yield return new WaitForSeconds(data[i].duration);
    //    }
    //    textDisplayEnded = true;
    //}

    //private IEnumerator DistroyUIText(Text[] texts, float fadeOutSeconds, AnimationCurve fadeOutCurve)
    //{
    //    textDestroyEnded = false;
    //    foreach(Text text in texts)
    //    {
    //        text.GetComponent<FadeText>().FadeOut(fadeOutSeconds, fadeOutCurve);
    //    }
    //    yield return new WaitForSeconds(fadeOutSeconds);
    //    foreach(Text text in texts)
    //    {
    //        Destroy(text.gameObject);
    //    }
    //    textDestroyEnded = true;
    //}
}
