using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public AudioSource audioSource;

    public AudioSource botAudioSource;


    public float fadeRate = .1f;
    public float volumeFaded = -20f;
    public float volumeNormal = 1f;

    private void Awake()
    {
        audioSource.Play(0);
    }

    private void Update()
    {
        // if null getb bot
        if (botAudioSource == null)
        {
            botAudioSource = GameObject.FindGameObjectWithTag("Bot").GetComponent<AudioSource>();
        }

        // if audio playing, fade until
        if (botAudioSource.isPlaying) FadeOut();
        else FadeIn();
    }

    void FadeIn()
    {
        if (audioSource.volume < volumeNormal)
            audioSource.volume += fadeRate;
    }
    void FadeOut()
    {
        if (audioSource.volume > volumeFaded)
            audioSource.volume -= fadeRate;
    }

}
