using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/**
 * 
 *  Control bot status and door
 * 
 */

public class BotManager : MonoBehaviour
{
    // references
    public TMP_Text botCountText;
    public int botsTotal;
    public GameObject[] sceneBots;
    public GameObject[] sceneDoors;
    public List<GameObject> allTheBots = new List<GameObject>();
    public List<string> foundBots = new List<string>();


    private void Awake()
    {
        sceneBots = GameObject.FindGameObjectsWithTag("Bot");
        sceneDoors = GameObject.FindGameObjectsWithTag("Door");
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //botCountText.text = foundBots.Count.ToString() + " / " + botsTotal.ToString() + " bots";
    }


}


// https://docs.unity3d.com/ScriptReference/PlayerPrefs.html