using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIContainer : SingletonMonoBehavior<UIContainer>
{
    public Canvas mainCanvas;
    public Canvas rightCanvas;
    public Canvas leftCanvas;

    private CanvasRenderer mainCanvasRenderer;
    private CanvasRenderer rightCanvasRenderer;
    private CanvasRenderer leftCanvasRenderer;

    private bool fadeOut = false;
    public bool IsFadeOut => fadeOut;
    void Start()
    {
        mainCanvasRenderer = mainCanvas.GetComponent<CanvasRenderer>();
        rightCanvasRenderer = rightCanvas.GetComponent<CanvasRenderer>();
        leftCanvasRenderer = leftCanvas.GetComponent<CanvasRenderer>();
    }

    public void SetCameras(CameraRig rig)
    {
        mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        mainCanvas.worldCamera = rig.front;
        rightCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        rightCanvas.worldCamera = rig.right;
        leftCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        leftCanvas.worldCamera = rig.left;
    }

    public void FadeOut(float seconds, AnimationCurve curve)
    {
        fadeOut = false;
        StartCoroutine(FadeOutCoroutine(seconds, curve));
    }

    private IEnumerator FadeOutCoroutine(float seconds, AnimationCurve curve)
    {
        float t = 1;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        while(t > 0)
        {
            float evT = curve.Evaluate(t);
            mainCanvasRenderer.SetAlpha(evT);
            rightCanvasRenderer.SetAlpha(evT);
            leftCanvasRenderer.SetAlpha(evT);
            t = Mathf.Clamp01(t - Time.deltaTime / seconds);
            yield return waitFrame;
        }
        fadeOut = true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
