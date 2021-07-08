using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public abstract class UIMedia : MonoBehaviour
{
    protected bool textDisplayEnded;
    protected bool textDestroyEnded;
    protected bool audioPlayEnded;
    protected bool isPlaying;
    public Canvas canvas;
    public AudioSource audioSource;
    public bool IsPlaying => isPlaying;
    public abstract IEnumerator Play();
    public abstract void SetData(Data data);
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
        Fade fade = obj.AddComponent<Fade>();
        fade.text = text;
        text.text = inputText.text;
        text.alpha = 0;
        text.transform.localPosition = new Vector3(text.transform.localPosition.x, text.transform.localPosition.y, 0);
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
            texts[i].GetComponent<Fade>().FadeIn(fadeInSeconds, fadeInCurve);
            if(data[i].duration != 0)
                yield return new WaitForSeconds(data[i].duration);
        }
        textDisplayEnded = true;
    }
    protected IEnumerator DestroyUIText(Text[] texts, float fadeOutSeconds, AnimationCurve fadeOutCurve)
    {
        textDestroyEnded = false;
        foreach (Text text in texts)
        {
            text.gameObject.GetComponent<Fade>().FadeOut(fadeOutSeconds, fadeOutCurve);
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
        if (audio != null)
        {
            yield return new WaitForSeconds(secondsBeforeAudio);
            UIContainer.Instance.PlayAudio(audio);
            //audioSource.PlayOneShot(audio);
            //var wait = new WaitWhile(() => audioSource.isPlaying);
            yield return UIContainer.Instance.AudioPlayEnded;
        }
        audioPlayEnded = true;
    }
}
