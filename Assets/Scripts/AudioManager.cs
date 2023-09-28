using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Audio;
public class AudioManager : MonoBehaviour
{
    AudioSource source, musicSource;
    public AudioClip[] badClips, goodClips, UIClips, music;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        musicSource = GetComponentInChildren<AudioSource>();
    }

    void Start()
    {
        source.clip = music[0];
        source.Play();
    }

    public void UISound()
    {
        source.clip = UIClips[0];
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
