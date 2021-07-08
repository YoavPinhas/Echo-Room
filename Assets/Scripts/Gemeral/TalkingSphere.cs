using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingSphere : IndestructibleSingleton<TalkingSphere>
{
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float goToPlaceSeconds = 2;
    [SerializeField] private float lowestBump = 0.5f;
    [SerializeField] private float highestBymp = 2;
    private MeshRenderer meshRenderer;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        
    }

    public void SetHeight(float t)
    {
        meshRenderer.material.SetFloat("_NormalPush", Mathf.Lerp(lowestBump, highestBymp, t));
    }
    void Update()
    {
        
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
