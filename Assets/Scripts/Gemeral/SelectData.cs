using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName ="SelectData.asset", menuName ="Data/Select from Options")]
public class SelectData : Data
{
    public float fadeInSeconds;
    public AnimationCurve fadeInCurve;
    public float fadeOutSeconds;
    public AnimationCurve fadeOutCurve;
    public AudioClip audio;
    public AudioClip listeningAudio;
    public AudioClip selectionAudio;
    public Color SelectionColor;
    public TextData[] textsBeforeSelection;
    public TextData[] selectionOptions;
    public DataEvaluator evaluator;
    public float secondsBeforePlayingAudio;
    public float secondsBeforeDisplayingText;
    public float secondsBeforeFadeOut;

    public override DataType GetDataType()
    {
        return DataType.ReleaseOption;
    }
}