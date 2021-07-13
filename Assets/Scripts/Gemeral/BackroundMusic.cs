using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackroundMusic : IndestructibleSingleton<BackroundMusic>
{
    public AudioClip music;
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
