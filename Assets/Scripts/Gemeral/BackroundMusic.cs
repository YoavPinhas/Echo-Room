using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackroundMusic : MonoBehaviour
{
    public AudioClip music;
    private static BackroundMusic instance;
    public static BackroundMusic Instance => instance;
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = music;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
