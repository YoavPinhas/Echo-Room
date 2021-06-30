using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ChooseFrustrationLevelData.asset", menuName = "Data/Frustration Level Data")]
public class ChooseFrustrationLevelData : Data
{
    public float fadeInSeconds;
    public AnimationCurve fadeInCurve;
    public float fadeOutSeconds;
    public AnimationCurve fadeOutCurve;
    public string option1SceneName;
    public string option2To10SceneName;
    public AudioClip audio;
    public TextData[] textsBeforeOptionsDisplayed;
    public TextData levelIs1;
    public TextData levelIs2;
    public TextData levelIs3;
    public TextData levelIs4;
    public TextData levelIs5;
    public TextData levelIs6;
    public TextData levelIs7;
    public TextData levelIs8;
    public TextData levelIs9;
    public TextData levelIs10;
    public float secondsBeforePlayingAudio;
    public float secondsBeforeDisplayingText;
    public override DataType GetDataType()
    {
        return DataType.ChooseFrustrationLevel;
    }
}
