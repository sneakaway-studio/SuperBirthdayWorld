using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioMixer masterMixer;
    public AudioSource botAudioSource;


    public AudioMixerSnapshot musicUp;
    public AudioMixerSnapshot musicDown;

    public float fadeRate = .1f;
    //public float volumeFaded = -20f;
    //public float volumeNormal = 1f;

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
        musicUp.TransitionTo(.1f);


        //if (audioSource.volume < volumeNormal)
        //    audioSource.volume += fadeRate;
    }
    void FadeOut()
    {

        musicDown.TransitionTo(.1f);

        //if (audioSource.volume > volumeFaded)
        //    audioSource.volume -= fadeRate;
    }

}
