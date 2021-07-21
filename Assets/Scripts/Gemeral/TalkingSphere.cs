using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingSphere : MonoBehaviour
{
    [SerializeField] private Transform sphere;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float goToPlaceSeconds = 2;
    [SerializeField] private float lowestBump = 0.5f;
    [SerializeField] private float highestBymp = 2;
    [SerializeField] private float fadeInSeconds = 1;
    [SerializeField] private float fadeOutSeconds = 1;
    [SerializeField] private AnimationCurve fadeinCurve;
    [SerializeField] private AnimationCurve fadeoutCurve;
    [SerializeField] bool fadeoutBackground;
    private MeshRenderer meshRenderer;
    [SerializeField]private MeshRenderer background;
    [SerializeField]private MeshRenderer foreground;
    public bool IsFadeIn {get; private set;} = true;
    public bool IsFadeOut {get; private set;} = true;
    private static TalkingSphere instance;
    public static TalkingSphere Instance => instance;
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
        meshRenderer = sphere.gameObject.GetComponent<MeshRenderer>();
        SetHeight(0);
    }

    public void SetHeight(float t)
    {
        meshRenderer.material.SetFloat("_NormalPush", Mathf.Lerp(lowestBump, highestBymp, t));
    }
    
    public void FadeIn()
    {
        meshRenderer.enabled = true;
        background.enabled = true;
        foreground.enabled = true;
        IsFadeIn = true;
        Color bc = background.material.color;
        bc.a = 1;
        background.material.color = bc;
        Color fc = foreground.material.color;
        fc.a = 1;
        foreground.material.color = fc;
        StartCoroutine(FadeInCoroutine(fc));
    }

    public void FadeOut()
    {
        Color bc = background.material.color;
        bc.a = 0;
        background.material.color = bc;
        Color fc = foreground.material.color;
        foreground.material.color = fc;
        meshRenderer.enabled = true;
        foreground.enabled = true;
        background.enabled = true;
        StartCoroutine(FadeOutCoroutine(fc, bc));
    }

    private IEnumerator FadeInCoroutine(Color color)
    {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        float t = 1;
        while(color.a != 0)
        {
            color.a = fadeinCurve.Evaluate(t);
            t = Mathf.Clamp01(t - Time.deltaTime / fadeInSeconds);
            foreground.material.color = color;
            yield return waitFrame;
        }
        IsFadeIn = false;
    }

    private IEnumerator FadeOutCoroutine(Color foregroundColor, Color backgroundColor)
    {
        IsFadeOut = true;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        float t = 0;
        while(foregroundColor.a != 1)
        {
            foregroundColor.a = fadeoutCurve.Evaluate(t);
            t = Mathf.Clamp01(t + Time.deltaTime / fadeOutSeconds);
            foreground.material.color = foregroundColor;
            yield return waitFrame;
        }
        meshRenderer.enabled = false;
        foreground.enabled = false;
        if (fadeoutBackground)
        {
            while (backgroundColor.a != 0)
            {
                backgroundColor.a = Mathf.Clamp01(backgroundColor.a - Time.deltaTime / fadeOutSeconds);
                background.material.color = backgroundColor;
                yield return waitFrame;
            }
        }
        background.enabled = false;
        yield return new WaitForSeconds(0.2f);        
        IsFadeOut = false;
    }

    public void GoToPosition()
    {
        StartCoroutine(GoToPositionCorouting());
    }
    private IEnumerator GoToPositionCorouting()
    {
        Tween tween = gameObject.AddComponent<Tween>();
        tween.StartTween(transform.position, targetPosition, goToPlaceSeconds, 1);
        yield return tween.TweenMotionCompleted;
        Destroy(tween);
    }
}
