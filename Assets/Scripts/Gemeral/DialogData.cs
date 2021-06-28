using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TextData
{
    public string text;
    public float duration;
    [Range(0, 1)] public float sceenX;
    [Range(0, 1)] public float sceenY;
    public int fontSize;
}

[System.Serializable]
public enum DataType { DialogData }

[System.Serializable]
public abstract class Data : ScriptableObject
{
    public abstract DataType GetDataType();
}

[System.Serializable]
[CreateAssetMenu(fileName ="DialogData.asset", menuName ="Data/Dialog Data")]
public class DialogData : Data
{
    public float fadeInSeconds;
    public AnimationCurve fadeInCurve;
    public float fadeOutSeconds;
    public AnimationCurve fadeOutCurve;
    public AudioClip audio;
    public TextData[] texts;
    public float secondsBeforePlayingAudio;
    public float secondsBeforeDisplayingText;
    public override DataType GetDataType()
    {
        return DataType.DialogData;
    }
}
