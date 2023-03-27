using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Bot : MonoBehaviour
{
    public bool triggered;

    public Transform hat;
    public Vector3 hatHide = new Vector3(0, 0, 0);
    public Vector3 hatShow;

    public Transform wheel;
    public Vector3 wheelHide = new Vector3(0, 0, 0);
    public Vector3 wheelShow;

    public Move_Float botFloat;
    public Move_Float bodyFloat;
    [SerializeField] private AudioSource audioSource;

    public GameObject radioWavesAnimation;


    private void OnValidate()
    {
        audioSource = GetComponent<AudioSource>();
        hat = transform.Find("BotRig/Hat").transform;
        hatShow = hat.localPosition;
        wheel = transform.Find("Wheel").transform;
        wheelShow = wheel.localPosition;
        // there are just two floats
        botFloat = GetComponent<Move_Float>();
        bodyFloat = transform.Find("BotRig").GetComponent<Move_Float>();
    }

    void Awake()
    {
        Trigger(false);
    }

    private void Update()
    {
        if (triggered && !audioSource.isPlaying)
        {
            Trigger(false);
        }

        if (audioSource.isPlaying)
            radioWavesAnimation.SetActive(true);
        else
            radioWavesAnimation.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!triggered && collision.transform.tag == "Player")
        {
            Trigger(true);
            PlayAudio();
        }
    }

    // startPos = the "hidden" position
    // endPos = the position in the editor before playing
    void Trigger(bool state)
    {
        if (!state)
        {
            triggered = false;
            //StartCoroutine(MoveObject(hat, hatHide, .75f));
            //StartCoroutine(MoveObject(wheel, wheelHide, .75f));
            StartCoroutine(MoveAndScaleObject(hat, hatHide, .5f, .75f));
            StartCoroutine(MoveAndScaleObject(wheel, wheelHide, .5f, .75f));
        }
        else
        {
            triggered = true;
            //StartCoroutine(MoveObject(hat, hatShow, .5f));
            //StartCoroutine(MoveObject(wheel, wheelShow, .5f));
            StartCoroutine(MoveAndScaleObject(hat, hatShow, 2f, .5f));
            StartCoroutine(MoveAndScaleObject(wheel, wheelShow, 2f, .5f));
        }

        if (botFloat.enabled)
            if (botFloat.isActiveAndEnabled) botFloat.floatingOn = state;

        if (bodyFloat.enabled)
            if (bodyFloat.isActiveAndEnabled) bodyFloat.floatingOn = state;


    }



    //IEnumerator MoveObject(Transform obj, Vector3 endPos, float seconds)
    //{
    //    Vector3 startPos = obj.transform.localPosition;
    //    float t = 0f;
    //    while (t <= 1.0)
    //    {
    //        t += Time.deltaTime / seconds;
    //        obj.localPosition = Vector3.Lerp(startPos, endPos, t);
    //        yield return null;
    //    }
    //}

    IEnumerator MoveAndScaleObject(Transform obj, Vector3 endPos, float endScale, float seconds)
    {
        Vector3 startPos = obj.transform.localPosition;
        Vector3 startScale = obj.transform.localScale;
        Vector3 endScaleV = new Vector3((float)(obj.transform.localScale.x * endScale), (float)(obj.transform.localScale.y * endScale), 1);
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            obj.localPosition = Vector3.Lerp(startPos, endPos, t);
            obj.localScale = Vector3.Lerp(startScale, endScaleV, t);
            yield return null;
        }
    }

    void PlayAudio()
    {
        //Debug.Log(audioSource.time);

        if (!audioSource.isPlaying)
        {
            audioSource.Play(0);
            //audioSource.UnPause();
            Debug.Log("Play: " + audioSource.time);
            radioWavesAnimation.SetActive(true);
        }
        //else
        //{
        //    audioSource.Pause();
        //    Debug.Log("Pause: " + audioSource.time);
        //}
    }


}