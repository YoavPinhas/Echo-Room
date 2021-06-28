using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIPlayer : MonoBehaviour
{
    [SerializeField] private Data[] dataToPlay;
    [SerializeField] private Canvas canvas;

    private Data currentData;
    private bool isPlaying = false;
    private bool thereIsText = false;
    private bool thereIsAudio = false;
    private int currentDataIndex = -1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaying)
        {
            currentDataIndex++;
            if (currentDataIndex >= dataToPlay.Length)
                return;
            currentData = dataToPlay[currentDataIndex];
            switch (currentData.GetDataType())
            {
                case DataType.DialogData:
                    StartCoroutine(PlayDialogData((DialogData)currentData));
                    return;
            }
        }
    }

    IEnumerator PlayDialogData(DialogData dialog)
    {
        isPlaying = true;
        if (dialog.audio != null)
            StartCoroutine(PlayDialogAudio(dialog));
        StartCoroutine(PlayDialogText(dialog));
        yield return new WaitUntil(() => thereIsText == false && thereIsAudio == false);
        isPlaying = false;
    }
    
    IEnumerator PlayDialogText(DialogData dialog)
    {
        thereIsText = true;
        yield return new WaitForSeconds(dialog.secondsBeforeDisplayingText);
        int textIndex = 0;
        List<GameObject> texts = new List<GameObject>();
        var width = canvas.pixelRect.width;
        var height = canvas.pixelRect.height;

        while (textIndex < dialog.texts.Length)
        {
            texts.Add(CreateDialogText(dialog.texts[textIndex], dialog.fadeInSeconds, dialog.fadeInCurve, width, height, $"text {textIndex}"));   
            yield return new WaitForSeconds(dialog.texts[textIndex].duration);
            textIndex++;
        }
        foreach (GameObject o in texts)
        {
            o.GetComponent<FadeText>().FadeOut(dialog.fadeOutSeconds, dialog.fadeOutCurve);
        }
        yield return new WaitForSeconds(dialog.fadeOutSeconds);
        foreach (GameObject o in texts)
        {
            Destroy(o);
        }
        texts = null;
        thereIsText = false;
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
    
    IEnumerator PlayDialogAudio(DialogData dialog)
    {
        thereIsAudio = true;
        yield return new WaitForSeconds(dialog.secondsBeforePlayingAudio);
        Debug.Log("Play Audio");
        thereIsAudio = false;
    }
}
