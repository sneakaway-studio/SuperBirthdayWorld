using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneFunctions
{
    /**
     *  Updated by GameManager
     */
    public static Indexer sceneIndex;
    public static List<string> sceneList;



    public static void LogActiveScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        Debug.Log("Active Scene name is: " + scene.name + "\nActive Scene index: " + scene.buildIndex);
    }

    public static void LoadScene(string sceneName)
    {
        LogActiveScene();
        if (!SceneManager.GetSceneByBuildIndex(0).IsValid())
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        LogActiveScene();
    }



    public static void LoadPrevScene()
    {
        SceneManager.LoadScene("Scenes/" + sceneList[sceneIndex.prev], LoadSceneMode.Additive);
    }
    public static void LoadNextScene()
    {
        SceneManager.LoadScene("Scenes/" + sceneList[sceneIndex.prev], LoadSceneMode.Additive);
    }




    /// <summary>
    /// "Pagination" struct with prev/current/next
    /// ORIGINAL from Sneakaway Utilities
    /// </summary>
    [System.Serializable]
    public struct Indexer
    {
        public int prev;
        public int current;
        public int next;
        public int count;
        // set default values
        public Indexer(int _count)
        {
            count = _count; // e.g. count = 10 = 10,0,1
            prev = count - 1;
            current = 0;
            next = 1;
        }
        // advance to next | prev index, update values
        public void NextIndex() => UpdateIndexes(next);
        public void PrevIndex() => UpdateIndexes(prev);

        // call after current has been set, to set / check values
        public void UpdateIndexes(int _current)
        {
            current = _current; // set current 
            if (current >= count) current = 0; // if should loop
            next = current + 1; // set next
            if (next >= count) next = 0; // if should loop
            prev = current - 1; // set prev
            if (prev < 0) prev = count - 1; // if should loop
        }
    }

}
