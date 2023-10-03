using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioThing : MonoBehaviour
{

    public bool good;
    void Awake()
    {
        PlayAudio();
    }
    void PlayAudio()
    {
        //references to necessary components
        AudioSource source = GetComponent<AudioSource>();
        AudioManager manager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
        

        if(good)
            source.clip = manager.goodClips[Random.Range(0,manager.goodClips.Length)];
        else
            source.clip = manager.badClips[Random.Range(0,manager.badClips.Length)];

        source.volume = 0.07f;

        source.Play();
        Destroy(gameObject, 2);
    }
}
