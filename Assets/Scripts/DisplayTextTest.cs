using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTextTest : MonoBehaviour
{
    public DataEvaluator evaluator;
    public void Display(string text)
    {
        Debug.Log(text);
        Debug.Log(evaluator.Choose(text));
    }
}
