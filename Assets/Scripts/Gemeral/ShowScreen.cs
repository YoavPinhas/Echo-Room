using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ShowScreen : MonoBehaviour
{
    private static ShowScreen instance = null;
    public static ShowScreen Instance => instance;

    [SerializeField] private VideoClip RoomIsFreeVideo;
    [SerializeField] private VideoClip RoomIsOccupiedVideo;
    [SerializeField] private VideoPlayer videoPlayer;
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        ShowRoomIsFreeVideo();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            ShowRoomIsOccupiedVideo();
    }
    public void ShowRoomIsFreeVideo()
    {
        videoPlayer.Stop();
        videoPlayer.clip = RoomIsFreeVideo;
        videoPlayer.Play();
    }

    public void ShowRoomIsOccupiedVideo()
    {
        videoPlayer.Stop();
        videoPlayer.clip = RoomIsOccupiedVideo;
        videoPlayer.Play();
    }
}
