using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopAnimation : MonoBehaviour
{
    private enum VisualState { Frustration, Controll, Fear}
    private enum LoopState { Opening, Closing, Open, Close }
    [System.Serializable]
    struct ReleaseStruct<T>
    {
        public T animation;
        public VisualState next;
    }
    [SerializeField] private ReleaseStruct<MandalaScene> releaseFrustrationAnimation;
    [SerializeField] private ReleaseStruct<ReleaseControlAnimation> releaseControlAnimation;
    [SerializeField] private ReleaseStruct<Scene2Animation> releaseFearAnimation;
    [SerializeField] private VisualState startLoopWith;

    [SerializeField] private float secondsToOpen = 3;
    [SerializeField] private float secondsBeforeClose = 3;
    [SerializeField] private float secondsToClose = 3;
    [SerializeField] private float secondsBeforeOpen = 3;

    [SerializeField] private AnimationCurve openCurve;
    [SerializeField] private AnimationCurve closeCurve;

    [SerializeField] private Camera frontCamera;
    [SerializeField] private Camera leftCamera;
    [SerializeField] private Camera rightCamera;

    private float counter = 0;
    private float curentSeconds;    
    private LoopState state;
    private VisualState visualState;
    private Vector3 leftCameraManadlaPosition;
    private Vector3 rightCameraManadlaPosition;
    private Quaternion leftCameraManadlaRotation;
    private Quaternion rightCameraManadlaRotation;

    void Start()
    {
        leftCameraManadlaPosition = leftCamera.transform.position;
        leftCameraManadlaRotation = leftCamera.transform.rotation;
        rightCameraManadlaPosition = rightCamera.transform.position;
        rightCameraManadlaRotation = rightCamera.transform.rotation;
        SetActive(VisualState.Frustration, false);
        SetActive(VisualState.Controll, false);
        SetActive(VisualState.Fear, false);
        visualState = startLoopWith;
        state = LoopState.Close;
        SetCurrentSeconds();
    }

    // Update is called once per frame
    void Update()
    {
        if(counter == 1)
        {
            CalculateState();
            SetCurrentSeconds();
            CalculateVisualState();
            return;
        }

        UpdateVisuals();

        counter += Time.deltaTime / curentSeconds;
    }

    private void UpdateVisuals()
    {
        if (state == LoopState.Open || state == LoopState.Close)
            return;
        if(state == LoopState.Opening)
        {
            SetDelta(counter, openCurve);
            return;
        }
        if(state == LoopState.Closing)
        {
            SetDelta(1 - counter, closeCurve);
            return;
        }
    }


    private void CalculateVisualState()
    {
        if (state == LoopState.Opening)
        {
            visualState = GetNextState();
            if(visualState == VisualState.Frustration)
            {
                rightCamera.transform.position = rightCameraManadlaPosition;
                leftCamera.transform.position = leftCameraManadlaPosition;
                rightCamera.transform.rotation = rightCameraManadlaRotation;
                leftCamera.transform.rotation = leftCameraManadlaRotation;
            }
            else
            {
                rightCamera.transform.position = frontCamera.transform.position;
                leftCamera.transform.position = frontCamera.transform.position;
                rightCamera.transform.rotation = frontCamera.transform.rotation * Quaternion.Euler(0,90,0);
                leftCamera.transform.rotation = frontCamera.transform.rotation * Quaternion.Euler(0, -90, 0);
            }
            SetActive(visualState, true);
            return;
        }
        if (state == LoopState.Close)
        {
            SetActive(visualState, false);
        }
    }

    private void CalculateState()
    {
        switch (state)
        {
            case LoopState.Opening:
                state = LoopState.Open;
                return;
            case LoopState.Closing:
                state = LoopState.Close;
                return;
            case LoopState.Open:
                state = LoopState.Closing;
                return;
            case LoopState.Close:
                state = LoopState.Opening;
                return;
        }
    }

    private void SetActive(VisualState state, bool onoff)
    {
        switch (state)
        {
            case VisualState.Frustration:
                releaseFrustrationAnimation.animation.gameObject.SetActive(onoff);
                break;
            case VisualState.Controll:
                releaseControlAnimation.animation.gameObject.SetActive(onoff);
                break;
            case VisualState.Fear:
                releaseFearAnimation.animation.gameObject.SetActive(onoff);
                break;
        }
    }

    private void SetCurrentSeconds()
    {
        counter = 0;
        switch (state)
        {
            case LoopState.Opening:
                curentSeconds = secondsToOpen;
                return;
            case LoopState.Closing:
                curentSeconds = secondsToClose;
                return;
            case LoopState.Open:
                curentSeconds = secondsBeforeClose;
                return;
            case LoopState.Close:
                curentSeconds = secondsBeforeOpen;
                return;
        }
    }

    private VisualState GetNextState()
    {
        switch (visualState)
        {
            case VisualState.Frustration:
                return releaseFrustrationAnimation.next;
            case VisualState.Controll:
                return releaseControlAnimation.next;
            case VisualState.Fear:
                return releaseFearAnimation.next;
        }
        return visualState;
    }

    private void SetDelta(float t, AnimationCurve curve)
    {
        float delta = curve.Evaluate(t);
        switch (visualState)
        {
            case VisualState.Frustration:
                releaseFrustrationAnimation.animation.OnMicrophonChangedLevel(delta);
                return;
            case VisualState.Controll:
                releaseControlAnimation.animation.OnMicrophonChangedLevel(delta);
                return;
            case VisualState.Fear:
                releaseFearAnimation.animation.OnMicrophoneLevelChanged(delta);
                return;
        }
    }
}
