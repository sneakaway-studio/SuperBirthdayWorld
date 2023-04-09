using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainThemeClip;
    public AudioClip dungeonThemeClip;
    public AudioClip robotDystopiaClip;
    public AudioSource audioSource;
    public AudioMixer masterMixer;
    public AudioSource botAudioSource;

    // https://www.kodeco.com/532-audio-tutorial-for-unity-the-audio-mixer
    // https://blog.studica.com/how-to-use-audio-mixers-in-unity-guide
    public AudioMixerSnapshot musicUp;
    public AudioMixerSnapshot musicDown;

    public float fadeRate = .1f;
    //public float volumeFaded = -20f;
    //public float volumeNormal = 1f;


    private void OnEnable()
    {
        EventManager.StartListening("UpdateTheme", UpdateTheme);
    }
    private void OnDisable()
    {
        EventManager.StopListening("UpdateTheme", UpdateTheme);
    }


    private void Awake()
    {
        audioSource.Play(0);
    }

    void UpdateTheme()
    {

        Debug.Log($"%%%%%  MusicManager.UpdateTheme() SceneControl.Instance.activeSceneLevel={SceneControl.Instance.activeSceneLevel}");

        switch (SceneControl.Instance.activeSceneLevel)
        {
            case 2:
                StartCoroutine(SwapAudioClip(dungeonThemeClip));
                break;
            case 3:
                StartCoroutine(SwapAudioClip(robotDystopiaClip));
                break;
            case 0:
            case 1:
            default:
                StartCoroutine(SwapAudioClip(mainThemeClip));
                break;
        }

    }

    private void Update()
    {
        // if null get bot
        if (botAudioSource == null)
            botAudioSource = GameObject.FindGameObjectWithTag("Bot").GetComponent<AudioSource>();

        // if bot audio begins playing, transition to snapshot to turn volume down
        if (botAudioSource.isPlaying)
            musicDown.TransitionTo(.1f);
        else
            musicUp.TransitionTo(.1f);
    }

    IEnumerator SwapAudioClip(AudioClip clip)
    {
        if (audioSource.clip == clip) yield break;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play(0);
        yield return new WaitForSeconds(.1f);
    }




}
