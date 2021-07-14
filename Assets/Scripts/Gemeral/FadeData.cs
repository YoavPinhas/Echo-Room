using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FadeOut.asset", menuName ="Data/FadeOut")]
public class FadeData : Data
{
    public bool fadeIn;
    public override DataType GetDataType()
    {
        return DataType.FadeOut;
    }
}
