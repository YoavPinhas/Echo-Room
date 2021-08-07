using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowScreenInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ShowScreen.Instance.ShowRoomIsFreeVideo();
    }

    public void OnProximityDetected()
    {
        ShowScreen.Instance.ShowRoomIsOccupiedVideo();
    }
}
