using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UIContainer : MonoBehaviour
{
    public Canvas mainCanvas;
    public Canvas rightCanvas;
    public Canvas leftCanvas;
    
    private CanvasRenderer mainCanvasRenderer;
    private CanvasRenderer rightCanvasRenderer;
    private CanvasRenderer leftCanvasRenderer;
    private AudioSource audioSource;
    private TalkingSphere talkingSphere;
    public WaitUntil AudioPlayEnded { get; private set; }

    private bool fadeOut = false;
    public bool IsFadeOut => fadeOut;
    public bool IsPlaying => audioSource.isPlaying;

    private static UIContainer instance;
    public static UIContainer Instance => instance;
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        mainCanvasRenderer = mainCanvas.GetComponent<CanvasRenderer>();
        rightCanvasRenderer = rightCanvas.GetComponent<CanvasRenderer>();
        leftCanvasRenderer = leftCanvas.GetComponent<CanvasRenderer>();
        audioSource = GetComponent<AudioSource>();
        AudioPlayEnded = new WaitUntil(() => !audioSource.isPlaying);
        talkingSphere = TalkingSphere.Instance;
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

    public void PlayAudio(AudioClip audio, bool withVisualisation = true)
    {
        if (audio == null)
            return;
        audioSource.PlayOneShot(audio);
        if(withVisualisation)
            StartCoroutine(VisualizeAudio());
    }

    private IEnumerator VisualizeAudio()
    {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        float multiplyer = 50f;
        float min = 0f;
        float max = 10f;
        while (audioSource.isPlaying)
        {
            //float rms = GetRMS();
            float t = Mathf.InverseLerp(min, max, GetRMS());
            talkingSphere.SetHeight(t);
            yield return waitFrame;
        }
    }

    private float GetRMS()
    {
        return GetRMSFromChannel(0) + GetRMSFromChannel(1);
    }

    private float GetRMSFromChannel(int channel)
    {
        float[] samples = new float[4096];
        audioSource.GetOutputData(samples, channel);
        float sum = 0;
        foreach (float f in samples)
        {
            sum += f * f;
        }
        return Mathf.Sqrt(sum);
    }
    void Update()
    {
    }
}
