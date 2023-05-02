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
        EventManager.StartListening("BotMessageStart", OnBotMessageStart);
        EventManager.StartListening("BotMessageEnd", OnBotMessageEnd);
    }
    private void OnDisable()
    {
        EventManager.StopListening("UpdateTheme", UpdateTheme);
        EventManager.StopListening("BotMessageStart", OnBotMessageStart);
        EventManager.StopListening("BotMessageEnd", OnBotMessageEnd);
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
            case 3:
                StartCoroutine(SwapAudioClip(dungeonThemeClip));
                break;
            case 4:
                StartCoroutine(SwapAudioClip(robotDystopiaClip));
                break;
            case 0:
            case 1:
            default:
                StartCoroutine(SwapAudioClip(mainThemeClip));
                break;
        }

    }

    IEnumerator SwapAudioClip(AudioClip clip)
    {
        if (audioSource.clip == clip) yield break;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play(0);
        yield return new WaitForSeconds(.1f);
    }



    void OnBotMessageStart()
    {
        //Debug.Log("BotMessageStart");
        // if bot audio begins playing, transition to snapshot to turn volume down
        musicDown.TransitionTo(.1f);
    }
    void OnBotMessageEnd()
    {
        //Debug.Log("BotMessageEnd");
        musicUp.TransitionTo(.1f);
    }

}
