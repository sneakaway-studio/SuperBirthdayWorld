using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class Bot : MonoBehaviour
{
    public bool messagePlaying;
    public int messageTimer;

    public Transform hat;
    public Vector3 hatHide = new Vector3(0, 0, 0);
    public Vector3 hatShow;

    public Transform wheel;
    public Vector3 wheelHide = new Vector3(0, 0, 0);
    public Vector3 wheelShow;

    public Move_Float botFloat;
    public Move_Float bodyFloat;

    public GameObject radioWavesAnimation;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextAsset textSource;

    // game intro message
    public Dictionary<string, string> messageDict = new Dictionary<string, string>();




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
        OnEndMessage(); // set bot to closed

        //PopulateGameIntroText();
    }



    private void Update()
    {
        // reset after audio finished
        if (messagePlaying && ++messageTimer > 100 && !audioSource.isPlaying && !textSourcePlaying) OnEndMessage();

        // show radio waves animation
        if (audioSource.isPlaying) radioWavesAnimation.SetActive(true);
        else radioWavesAnimation.SetActive(false);

        // check status of floating animation
        if (botFloat.enabled)
            if (botFloat.isActiveAndEnabled)
                botFloat.floatingOn = messagePlaying;
        if (bodyFloat.enabled)
            if (bodyFloat.isActiveAndEnabled)
                bodyFloat.floatingOn = messagePlaying;
    }

    // OnTriggerEnter2D is in child obj






    /////////////////////// TRIGGER ///////////////////////


    // startPos = the "hidden" position
    // endPos = the position in the editor before playing
    public void OnShowMessage()
    {
        // already playing
        if (!messagePlaying)
        {
            messagePlaying = true;
            messageTimer = 0;

            //StartCoroutine(MoveObject(hat, hatShow, .5f));
            //StartCoroutine(MoveObject(wheel, wheelShow, .5f));
            StartCoroutine(MoveAndScaleObject(hat, hatShow, 2f, .5f));
            StartCoroutine(MoveAndScaleObject(wheel, wheelShow, 2f, .5f));

            PlayMessage();
        }
    }

    public void OnEndMessage()
    {
        messagePlaying = false;

        //StartCoroutine(MoveObject(hat, hatHide, .75f));
        //StartCoroutine(MoveObject(wheel, wheelHide, .75f));
        StartCoroutine(MoveAndScaleObject(hat, hatHide, .5f, .75f));
        StartCoroutine(MoveAndScaleObject(wheel, wheelHide, .5f, .75f));
    }



    /////////////////////// ANIMATIONS ///////////////////////


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
        //Debug.Log($"MoveAndScaleObject() {obj.name} {endPos} {endScale} {seconds}");
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



    /////////////////////// MESSAGES ///////////////////////

    bool textSourcePlaying = false;

    void PlayMessage()
    {
        if (textSource != null)
        {
            StartCoroutine(DisplayTextMessage());
        }
        // if the message is audio
        else if (audioSource.clip != null) PlayAudio();
    }

    void PlayAudio()
    {
        //Debug.Log(audioSource.time);

        // if not already playing
        if (!audioSource.isPlaying)
        {
            audioSource.Play(0);
            //audioSource.UnPause();
            Debug.Log("Play: " + audioSource.time);
            radioWavesAnimation.SetActive(true);
        }
    }

    IEnumerator DisplayTextMessage()
    {
        // the introduction version
        //string message = "";
        //if (messageDict.TryGetValue(SceneControl.Instance.activeSceneLevelString, out message))
        //    SceneControl.Instance.messageTextTeletyper.AddText(message, this);

        if (textSourcePlaying) yield return null;
        // so it can only be called once
        textSourcePlaying = true;

        // start the teletyper
        SceneControl.Instance.messageTextTeletyper.AddText(textSource.text, this);

        // and wait until it is done
        while (textSourcePlaying)
        {
            Debug.Log("Waiting for end of text message");

            yield return new WaitForSeconds(1);
        }
    }









    void PopulateGameIntroText()
    {
        messageDict["0-0"] = "Joelle has a big birthday coming up and we wanted to do something memorable to celebrate it.";
        messageDict["1-1"] = "In this simple 2D platformer Joelle plays herself to unlock recorded birthday messages from friends and family. ";
        messageDict["1-2"] = "The Joelle character updates are by our daughter, Sophia, and Joelle's brother, John created the music. ";
        messageDict["1-3"] = "With the basics finished, you are invited to record a message that will be played by one of the robots in the game. ";
        messageDict["1-4"] = "You can simply say 'Happy Birthday' or record a story or memory that you shared with her. ";
        messageDict["2-1"] = "She said the only thing I was allowed to do for her birthday was a sappy card, so feel free to lay it on :-)! See instructions in the email to record a message. Thanks, Owen";
    }

}