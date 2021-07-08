using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class Fade : MonoBehaviour
{
    public MaskableGraphic text;
    private WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
    private Color color;
    public void FadeIn(float seconds, AnimationCurve curve)
    {
        if (seconds < 0)
            return;
        StartCoroutine(FadeInEnomerator(seconds, curve));

    }

    private IEnumerator FadeInEnomerator(float seconds, AnimationCurve curve)
    {
        float counter = 0;
        color = text.color;
        color.a = 1;
        text.color = color;
        while (counter != seconds)
        {
            counter = Mathf.Clamp(counter + Time.deltaTime, 0, seconds);
            float t = counter / seconds;
            color.a = curve.Evaluate(t);
            text.color = color;
            yield return endOfFrame;
        }
    }


    public void FadeOut(float seconds, AnimationCurve curve)
    {
        if (seconds < 0)
            return;
        StartCoroutine(FadeOutEnomerator(seconds, curve));
    }

    private IEnumerator FadeOutEnomerator(float seconds, AnimationCurve curve)
    {
        float counter = 0;
        color = text.color;
        color.a = 1;
        text.color = color;
        while (counter != seconds)
        {
            counter = Mathf.Clamp(counter + Time.deltaTime, 0, seconds);
            float t = 1 - counter / seconds;
            color.a = curve.Evaluate(t);
            text.color = color;
            yield return endOfFrame;
        }
    }
}
