using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

using Text = TMPro.TextMeshProUGUI;
[RequireComponent(typeof(AudioSource))]
public class UIPlayer : MonoBehaviour
{
    [SerializeField] private Data[] dataToPlay;
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
                break;
            case DataType.StressLevel:
                currentMedia = gameObject.AddComponent<UIChooseStressMedia>();
                break;
            case DataType.ReleaseOption:
                currentMedia = gameObject.AddComponent<UISelectionMedia>();
                break;
            case DataType.FadeOut:
                currentMedia = gameObject.AddComponent<UIFadeMedia>();
                break;
            case DataType.StartScream:
                currentMedia = gameObject.AddComponent<UIStartScreamMedia>();
                break;
            case DataType.LoadScene:
                SceneManager.LoadScene(((LoadSceneData)data).sceneName);
            return;
        }
        currentMedia.SetData(data);
        currentMedia.audioSource = audioSource;
    }

}
