using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 *  Control bot status and door
 * 
 */

public class BotManager : MonoBehaviour
{
    // references
    public GameObject[] sceneBots;
    public GameObject[] sceneDoors;
    public List<GameObject> allTheBots = new List<GameObject>();


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

    }
}
