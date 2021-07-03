using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ReleaseData
{
    public TextData text;
    public string associatedSceneName;
}

[CreateAssetMenu(fileName ="ReleaseOptions.asset", menuName ="Data/Release Options")]
public class ReleaseOptions : Data
{
    public float fadeInSeconds;
    public AnimationCurve fadeInCurve;
    public float fadeOutSeconds;
    public AnimationCurve fadeOutCurve;
    public string option1SceneName;
    public ReleaseData[] options;
    public AudioClip audio;
    public DataEvaluator evaluator;
    public override DataType GetDataType()
    {
        return DataType.ReleaseOption;
    }
}