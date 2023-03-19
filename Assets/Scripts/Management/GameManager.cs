using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  GameManager
 *  - Attached to ManagerScene (loaded every time, just once)
 *  - The place to input all game data (may need to push to static class?)
 *  - Bot # corresponds to sceneIndex number
 */

public class GameManager : MonoBehaviour
{
    [Tooltip("Prev/Current/Next)")]
    public SceneFunctions.Indexer sceneIndex;

    public List<string> sceneList = new List<string>();

    private void OnValidate()
    {
        SceneFunctions.LogActiveScene();

        // always update sceneIndex
        sceneIndex = new SceneFunctions.Indexer(sceneList.Count);

        // always update in SceneFunctions
        SceneFunctions.sceneIndex = sceneIndex;
        SceneFunctions.sceneList = sceneList;

    }

    private void Awake()
    {

    }


    void DisplaySceneName()
    {

    }







    //public List<int> bots;

    //public List<int> UpdateBots(int num = -1)
    //{
    //    if (num > -1)
    //    {
    //        if (!bots.Contains(num))
    //        {
    //            bots.Add(num);
    //        }
    //    }
    //    bots.Sort();
    //    Debug.Log(bots);
    //    return bots;
    //}

    //private void Update()
    //{

    //}


}
