using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName ="ReleaseOptions.asset", menuName ="Data/Release Options")]
public class ReleaseOptionsData : Data
{
    public float fadeInSeconds;
    public AnimationCurve fadeInCurve;
    public float fadeOutSeconds;
    public AnimationCurve fadeOutCurve;
    public string option1SceneName;
    public AudioClip audio;
    public TextData[] texts;
    public DataEvaluator evaluator;
    public float secondsBeforePlayingAudio;
    public float secondsBeforeDisplayingText;

    public override DataType GetDataType()
    {
        return DataType.ReleaseOption;
    }
}