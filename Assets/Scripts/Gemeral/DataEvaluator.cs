using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OptionContainer
{
    public string optionName;
    public OptionData option;
}

[CreateAssetMenu(fileName ="Evaluator", menuName ="Data/Options Evaluator")]
public class DataEvaluator : ScriptableObject
{
    public OptionContainer[] Options;
    public string Choose(string sentence)
    {
        if (Options == null || Options.Length == 0)
            return null;
        
            sentence = sentence.Replace(".", string.Empty);
            sentence = sentence.Replace(",", string.Empty);
            sentence = sentence.Replace("\\", string.Empty);
            sentence = sentence.Replace("/", string.Empty);
            sentence = sentence.Replace(";", string.Empty);
            sentence = sentence.Replace(":", string.Empty);
            sentence = sentence.Replace("+", string.Empty);
            sentence = sentence.Replace("-", string.Empty);
        
        int maxIndex = 0;
        float max = Options[0].option.Evaluate(sentence);
        for (int i = 1; i < Options.Length; i++)
        {
            float score = Options[i].option.Evaluate(sentence);
            if (score > max)
            {
                maxIndex = i;
                max = score;
            }    
        }
        return Options[maxIndex].optionName;
    }

}
