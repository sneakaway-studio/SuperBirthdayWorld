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
    public bool volumeDown;


    private void OnEnable()
    {
        EventManager.StartListening("UpdateTheme", UpdateTheme);
        EventManager.StartListening("TurnMusicDown", TurnMusicDown);
        EventManager.StartListening("TurnMusicUp", TurnMusicUp);
    }
    private void OnDisable()
    {
        EventManager.StopListening("UpdateTheme", UpdateTheme);
        EventManager.StopListening("TurnMusicDown", TurnMusicDown);
        EventManager.StopListening("TurnMusicUp", TurnMusicUp);
    }


    private void Awake()
    {
        audioSource.Play(0);
    }

    void UpdateTheme()
    {
        Debug.Log($"MusicManager.UpdateTheme() SceneControl.Instance.activeSceneLevel={SceneControl.Instance.activeSceneLevel}");

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
        Debug.Log("MusicManager.SwapAudioClip() clip = " + clip.name);
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play(0);
        yield return new WaitForSeconds(.1f);
    }

    public void TurnMusicDown()
    {
        Debug.Log("MusicManager.TurnMusicDown()");
        // if bot audio begins playing, transition to snapshot to turn volume down
        musicDown.TransitionTo(.1f);
        volumeDown = true;
    }
    public void TurnMusicUp()
    {
        Debug.Log("MusicManager.TurnMusicUp()");
        musicUp.TransitionTo(.1f);
        volumeDown = false;
    }

}
