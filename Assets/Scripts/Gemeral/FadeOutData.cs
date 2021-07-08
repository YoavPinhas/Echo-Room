using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FadeOut.asset", menuName ="Data/FadeOut")]
public class FadeOutData : Data
{
    public float fadeSeconds;
    public AnimationCurve fadeCurve;
    public override DataType GetDataType()
    {
        return DataType.FadeOut;
    }
}
