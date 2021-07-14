using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="LoadScene", menuName ="Data/Load Scene Data")]
public class LoadSceneData : Data
{
    public string sceneName;
    public override DataType GetDataType()
    {
        return DataType.LoadScene;
    }
}
