using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeText : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    private WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
    public void FadeIn(float seconds, AnimationCurve curve)
    {
        if (seconds < 0)
            return;
        StartCoroutine(FadeInEnomerator(seconds, curve));

    }

    private IEnumerator FadeInEnomerator(float seconds, AnimationCurve curve)
    {
        float counter = 0;
        text.alpha = 1;
        while (counter != seconds)
        {
            counter = Mathf.Clamp(counter + Time.deltaTime, 0, seconds);
            float t = counter / seconds;
            text.alpha = curve.Evaluate(t);
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
        text.alpha = 1;
        while (counter != seconds)
        {
            counter = Mathf.Clamp(counter + Time.deltaTime, 0, seconds);
            float t = 1 - counter / seconds;
            text.alpha = curve.Evaluate(t);
            yield return endOfFrame;
        }
    }
}
