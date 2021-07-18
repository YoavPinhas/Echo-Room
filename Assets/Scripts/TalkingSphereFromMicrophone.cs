using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingSphereFromMicrophone : MonoBehaviour
{
    [SerializeField] private float lowestBump = 0.5f;
    [SerializeField] private float highestBymp = 2;
    public MeshRenderer foreGround;
    public float fadeoutSeconds = 2;
    private MeshRenderer meshRenderer;
    private float height;
    private float speed = 10;
    public bool IsFadeOut { get; private set; } = false;
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        SetHeight(0);
        Color c = foreGround.material.color;
        c.a = 0;
        foreGround.material.color = c;
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCorouting());
    }

    private IEnumerator FadeOutCorouting()
    {
        IsFadeOut = false;
        float t = 0;
        Color color = foreGround.material.color;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        while(t < 1)
        {
            color.a = t;
            foreGround.material.color = color;
            t = Mathf.Clamp01(t + Time.deltaTime / fadeoutSeconds);
            yield return waitFrame;
        }
        color.a = 1;
        foreGround.material.color = color;
        IsFadeOut = true;
    }
    public void SetHeight(float t)
    {
        float destHeight = Mathf.Lerp(lowestBump, highestBymp, t);
        height = Mathf.Lerp(height, destHeight, Time.deltaTime * speed);
        meshRenderer.material.SetFloat("_NormalPush", height);
    }
}
