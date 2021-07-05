using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

public abstract class UIMedia : MonoBehaviour
{
    protected bool textDisplayEnded;
    protected bool textDestroyEnded;
    protected bool audioPlayEnded;
    protected bool isPlaying;
    public Data data;
    public Canvas canvas;
    public AudioSource audioSource;
    public bool IsPlaying => isPlaying;
    public abstract IEnumerator Play();
    protected Text[] CreateUITextsArray(TextData[] inputTexts)
    {
        if (inputTexts == null || inputTexts.Length == 0)
        {
            Debug.LogError("Create UI Text Array need to get valid non empty array.");
            return null;
        }
        var width = canvas.pixelRect.width;
        var height = canvas.pixelRect.height;
        Text[] result = new Text[inputTexts.Length];
        for (int i = 0; i < inputTexts.Length; i++)
        {
            result[i] = CreateUIText(inputTexts[i], width, height, $"Text {i}");
        }
        return result;
    }
    protected Text CreateUIText(TextData inputText, float width, float height, string name = "")
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(canvas.transform);
        Text text = obj.AddComponent<Text>();
        var posX = (inputText.sceenX - 0.5f) * width;
        var posY = (inputText.sceenY - 0.5f) * height;
        text.rectTransform.anchoredPosition = new Vector3(posX, posY, 0);
        text.font = inputText.font;
        text.fontSize = inputText.fontSize;
        text.color = inputText.color;
        text.autoSizeTextContainer = true;
        FadeText fade = obj.AddComponent<FadeText>();
        fade.text = text;
        text.text = inputText.text;
        text.alpha = 0;
        return text;
    }
    protected IEnumerator DisplayUITexts(Text[] texts, TextData[] data, float fadeInSeconds, AnimationCurve fadeInCurve, float delay)
    {
        if (texts == null || data == null || texts.Length == 0 || data.Length == 0 || texts.Length != data.Length)
        {
            Debug.LogError("Texts and data must be a valid arrays with same length for DisplayUIText to work.");
            yield break;
        }
        yield return new WaitForSeconds(delay);
        textDisplayEnded = false;
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].GetComponent<FadeText>().FadeIn(fadeInSeconds, fadeInCurve);
            yield return new WaitForSeconds(data[i].duration);
        }
        textDisplayEnded = true;
    }
    protected IEnumerator DestroyUIText(Text[] texts, float fadeOutSeconds, AnimationCurve fadeOutCurve)
    {
        textDestroyEnded = false;
        foreach (Text text in texts)
        {
            text.GetComponent<FadeText>().FadeOut(fadeOutSeconds, fadeOutCurve);
        }
        yield return new WaitForSeconds(fadeOutSeconds);
        foreach (Text text in texts)
        {
            Destroy(text.gameObject);
        }
        textDestroyEnded = true;
    }
    protected IEnumerator PlayAudio(AudioClip audio, float secondsBeforeAudio)
    {
        audioPlayEnded = false;
        yield return new WaitForSeconds(secondsBeforeAudio);
        audioSource.PlayOneShot(audio);
        var wait = new WaitWhile(() => audioSource.isPlaying);
        yield return wait;
        audioPlayEnded = true;
    }
}
