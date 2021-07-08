using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 finalPosition;
    private float secondsToComplete;
    private float tIsComplete;
    private float t;
    public WaitUntil TweenMotionCompleted;

    public void StartTween(Vector3 start, Vector3 destination, float seconds, float complete)
    {
        startPosition = start;
        finalPosition = destination;
        secondsToComplete = seconds;
        t = 0;
        tIsComplete = Mathf.Clamp01(complete);
        transform.position = startPosition;
        TweenMotionCompleted = new WaitUntil(() => t >= tIsComplete);
        StartCoroutine(TweenMovement());
    }
    private IEnumerator TweenMovement()
    {
        Vector3 direction = finalPosition - startPosition;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        while (t < 1)
        {
            yield return waitFrame;
            t = Mathf.Clamp01(t + Time.deltaTime / secondsToComplete);
            transform.position = startPosition + t * direction;
        }
    }
}
