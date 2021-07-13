using UnityEngine;

[CreateAssetMenu(fileName = "StartScream.asset", menuName = "Data/Start Scream")]
public class StartScreamData : Data
{
    public float maxSeconds;
    public override DataType GetDataType()
    {
        return DataType.StartScream;
    }
}
