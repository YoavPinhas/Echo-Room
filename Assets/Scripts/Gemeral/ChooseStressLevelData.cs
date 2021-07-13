using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ChooseFrustrationLevelData.asset", menuName = "Data/Frustration Level Data")]
public class ChooseStressLevelData : Data
{
    public float fadeInSeconds;
    public AnimationCurve fadeInCurve;
    public float fadeOutSeconds;
    public AnimationCurve fadeOutCurve;
    public string option1SceneName;
    public string options2To10SceneName;
    public AudioClip audio;
    public AudioClip selectionAudio;
    public TextData[] textsBeforeOptionsDisplayed;
    public TextData numbersFormat;
    public Texture2D numbersDecorator;
    public Color selectedColor;
    public DataEvaluator evaluator;
    public float secondsBeforePlayingAudio;
    public float secondsBeforeDisplayingText;
    public float secondsAfterSelection;
    public int maxRecordingSeconds;
    public GameObject displayResultPrefab;
    public AudioClip unrecognizedAudio;
    public TextData[] unrecognizedText;
    [Range(1, 10)] public int defaultSelection;

    public override DataType GetDataType()
    {
        return DataType.StressLevel;
    }
}
