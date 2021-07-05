using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Text = TMPro.TextMeshProUGUI;
[RequireComponent(typeof(AudioSource))]
public class UIPlayer : MonoBehaviour
{
    [SerializeField] private Data[] dataToPlay;
    [SerializeField] private Canvas canvas;
    private AudioSource audioSource;

    private bool isPlaying = false;
    private bool thereIsText = false;
    private bool audioIsEnded = false;
    private bool textDisplayEnded = false;
    private bool textDestroyEnded = false;
    private int currentDataIndex = -1;
    private WaitWhile waitIfAudioIsPlaying;
    private Dictionary<Data, UIMedia> uiMedia = new Dictionary<Data, UIMedia>();
    UIMedia currentMedia = null;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        waitIfAudioIsPlaying = new WaitWhile(() => audioSource.isPlaying);
    }

    void Update()
    {
        if (currentMedia == null || !currentMedia.IsPlaying)
        {
            currentDataIndex++;
            if (currentDataIndex >= dataToPlay.Length)
                return;
            CreateMedia(dataToPlay[currentDataIndex]);
            StartCoroutine(currentMedia.Play());
        }
    }

    private void CreateMedia(Data data)
    {
        Destroy(currentMedia);
        switch (data.GetDataType())
        {
            case DataType.DialogData:
                currentMedia = gameObject.AddComponent<UIDialogMedia>();
                currentMedia.data = (DialogData)data;
                break;
            case DataType.StressLevel:
                currentMedia = gameObject.AddComponent<UIChooseStressMedia>();
                currentMedia.data = (ChooseStressLevelData)data;
                break;
            case DataType.ReleaseOption:
                break;
        }
        currentMedia.audioSource = audioSource;
        currentMedia.canvas = canvas;
    }

}
