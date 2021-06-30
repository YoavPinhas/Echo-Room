using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="OptionData.asset" ,menuName ="Data/Option Data")]
public class OptionData : ScriptableObject
{
    public string[] sentences;
    public string[] keyWords;

    public float Evaluate(string text)
    {
        return Evaluate(text, text.Split(' '));
    }
    private float Evaluate(string originalText, string[] words) 
    {
        if (databaseContainsTheSentence(originalText))
            return 100;

        float sentencesSum = 0;
        foreach(string sentence in sentences)
        {
            string[] s = sentence.Split(' ');
            if (IsSubText(s, words) || IsSubText(words, s) || MatchOrder(s,words))
            {
                sentencesSum++;
            }
        }
        float keyordsSum = 0;
        foreach(string keyword in keyWords)
        {
            if (words.Contains(keyword))
                keyordsSum++;
        }

        return Mathf.Clamp(((float)(sentencesSum * 30 + keyordsSum * 70) / (sentences.Length + keyWords.Length)), 0, 100);

        //float sum1 = 0;
        //foreach(string sentence in sentences)
        //{
        //    string[] s = sentence.Split(' ');
        //    if(IsSubText(s, words) || IsSubText(words, s))
        //    {
        //        int wordCount = words.Length;
        //        int sentenceLength = s.Length;
        //        float score = 100f / sentenceLength;
        //        int keywordsCount = 0;
        //        foreach (string key in keyWords)
        //            if (words.Contains(key))
        //                keywordsCount++;
        //        sum1 += score * (Mathf.Abs(wordCount - sentenceLength) + keywordsCount);
        //    }
        //}

        //float sum2 = 0;
        //foreach(string sentence in sentences)
        //{
        //    string[] sWords = sentence.Split(' ');
        //    foreach(string sWord in sWords)
        //    {
        //        foreach(string word in words)
        //        {
        //            if (word == sWord)
        //                sum2++;
        //        }
        //    }
        //}
        //return (0.6f * sum1 + 0.4f * sum2) / sentences.Length;
    }

    private bool databaseContainsTheSentence(string text)
    {
        if (keyWords.Contains(text) || sentences.Contains(text))
            return true;
        return false;
    }

    private static bool IsSubText(string[] text, string[] sub)
    {
        int tl = text.Length;
        int sl = sub.Length;
        if (sl > tl || sl == 0 || tl == 0)
            return false;

        int j = 0;
        for (int i = 0; i < tl; i++)
        {
            if (text[i] == sub[j])
            {
                j++;
            }

            if (j == sl)
                return true;
        }
        return false;
    }


    private bool MatchOrder(string[] A, string[] B)
    {
        int indexA = 0;
        int prevIndexB = 0;
        int indexB = 0;
        int matchCount = 0;
        bool hasMatch = false;
        while (indexA < A.Length && prevIndexB < B.Length)
        {
            hasMatch = false;
            while (indexB < B.Length)
            {
                if (A[indexA] == B[indexB])
                {
                    matchCount++;
                    prevIndexB = indexB;
                    hasMatch = true;
                    break;
                }
                indexB++;
            }
            indexA++;
            if (!hasMatch)
            {
                indexB = prevIndexB;
            }
        }
        return matchCount > 3;
    }
}
