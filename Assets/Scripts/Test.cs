using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX.Utility;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
   public void SayHi()
   {
       Debug.Log("Hi :)");  
   }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShowScreen.Instance.ShowRoomIsOccupiedVideo();
            SceneManager.LoadScene("Opening Scene");
        }
    }
}
