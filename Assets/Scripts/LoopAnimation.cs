using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoopAnimation : MonoBehaviour
{
    private enum EffectType { Frustration, Controll, Fear}
    private enum LoopState { Opening, Closing, Open, Close }

    [SerializeField] private float secondsToOpen = 3;
    [SerializeField] private float secondsBeforeClose = 3;
    [SerializeField] private float secondsToClose = 3;
    [SerializeField] private float secondsBeforeOpen = 3;

    [SerializeField] private AnimationCurve openCurve;
    [SerializeField] private AnimationCurve closeCurve;
    [SerializeField] private string nextScene;

    [SerializeField] private EffectType effectType;
    private EffectScene effect;

    private float counter = 0;
    private float curentSeconds;    
    private LoopState state;


    void Start()
    {
        switch (effectType)
        {
            case EffectType.Frustration:
                effect = FindObjectOfType<MandalaScene>();
                break;
            case EffectType.Controll:
                effect = FindObjectOfType<ReleaseControlAnimation>();
                break;
            case EffectType.Fear:
                effect = FindObjectOfType<Scene2Animation>();
                break;
        }
        effect = FindObjectOfType<EffectScene>();
        state = LoopState.Close;
        SetCurrentSeconds();
    }

    // Update is called once per frame
    void Update()
    {
        if(counter == 1)
        {
            CalculateState();
            if(state == LoopState.Opening)
            {
                SceneManager.LoadScene(nextScene);
                return;
            }
            SetCurrentSeconds();
            return;
        }

        UpdateVisuals();

        counter += Time.deltaTime / curentSeconds;
    }

    private void UpdateVisuals()
    {
        switch (state)
        {
            case LoopState.Opening:
                effect.OnMicrophonChangedLevel(openCurve.Evaluate(counter));
                break;
            case LoopState.Closing:
                effect.OnMicrophonChangedLevel(openCurve.Evaluate(1-counter));
                break;
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

}
