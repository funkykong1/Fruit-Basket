using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource source, musicSource;
    public AudioClip[] badClips, goodClips, UIClips, music, groundClips;

    void Awake()
    {
        source = this.gameObject.GetComponent<AudioSource>();
        musicSource = GameObject.Find("Music").GetComponent<AudioSource>();
    }

    void Start()
    {
        musicSource.clip = music[0];
        musicSource.Play();
    }

    public void UISound()
    {
        source.clip = UIClips[0];
        source.Play();
    }
}
