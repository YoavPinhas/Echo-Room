using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ChooseFrustrationLevelData.asset", menuName = "Data/Frustration Level Data")]
public class ChooseStressLevelData : Data
{
    public float fadeInSeconds;
    public AnimationCurve fadeInCurve;
    public float fadeOutSeconds;
    public AnimationCurve fadeOutCurve;
    public string nextSceneName;
    public AudioClip audio;
    public TextData[] textsBeforeOptionsDisplayed;
    public TextData numbersFormat;
    public DataEvaluator evaluator;
    public float secondsBeforePlayingAudio;
    public float secondsBeforeDisplayingText;

    public override DataType GetDataType()
    {
        return DataType.StressLevel;
    }
}
