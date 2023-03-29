using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneControl : MonoBehaviour
{
    [Header("Scene Data")]

    [Tooltip("Number of scenes in the build")]
    public int sceneCount;
    [Tooltip("List of scene names in the build")]
    public List<string> sceneList;
    [Tooltip("Index of prev / current / next scenes")]
    public SceneControl_Indexer indexer;

    [Header("Current Scene")]

    [Tooltip("Active scene")]
    public Scene activeScene;
    [Tooltip("Active scene name")]
    public string activeSceneName;
    [Tooltip("Active scene index")]
    public int activeSceneIndex;
    [Tooltip("Active scene index")]
    public bool activeSceneLoaded;


    [Tooltip("Index of prev / current / next scenes")]
    public TMP_Text sceneDebugText;



    private void Awake()
    {
        if (sceneList == null || sceneList.Count < 1) InitSceneControl();
        UpdateActiveSceneData();
    }

    void InitSceneControl()
    {
        Debug.Log($"^^^ 0 ^^^ SceneControl.InitSceneControl()");
        // if the sceneList has not been created
        sceneCount = SceneManager.sceneCountInBuildSettings;
        indexer = new SceneControl_Indexer();
        indexer.Reset(sceneCount);
        sceneList = GetScenesInBuild();
    }

    void UpdateActiveSceneData()
    {
        Debug.Log($"^^^ 1 ^^^ SceneControl.UpdateActiveSceneData()");
        // always use the current scene to update the indexer
        activeScene = SceneManager.GetActiveScene();
        activeSceneName = activeScene.name;
        activeSceneIndex = activeScene.buildIndex;
        activeSceneLoaded = activeScene.isLoaded;
        indexer.UpdateIndexes(activeSceneIndex);
        sceneDebugText.text = $"prev={sceneList[indexer.prev]} ({indexer.prev}) | " +
            $"current={sceneList[indexer.current]} ({indexer.current}) | " +
            $"next={sceneList[indexer.next]} ({indexer.next})";
    }



    ////////////////////////////////////////////////////// 
    ///////////////////// LISTENERS //////////////////////
    //////////////////////////////////////////////////////


    // OnEnable / OnDisable listeners
    private void OnEnable()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        //SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    private void OnDisable()
    {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        //SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // prev, current, next
    public void LoadPreviousScene() => LoadScene(sceneList[indexer.prev]);
    public void LoadCurrentScene() => LoadScene(sceneList[indexer.current]);
    public void LoadNextScene() => LoadScene(sceneList[indexer.next]);
    // first, last
    public void LoadFirstScene() => LoadScene(sceneList[indexer.next]);
    public void LoadLastScene() => LoadScene(sceneList[indexer.next]);
    // by name
    public void LoadScene(string _name)
    {
        // also could use async version?
        SceneManager.LoadScene(_name);
    }

    /// <summary>When the active scene has changed (after OnSceneLoaded) (called from delegate)</summary>
    void OnActiveSceneChanged(Scene previousActive, Scene newActive)
    {
        Debug.Log($"^^^ 1 ^^^ OnActiveSceneChanged() previousActive.name='{previousActive.name}', newActive.name={newActive.name}");
        UpdateActiveSceneData();
    }


    ////////////////////////////////////////////////////// 
    /////////////////// SCENE STATUS /////////////////////
    //////////////////////////////////////////////////////


    private List<string> GetScenesInBuild()
    {
        List<string> scenes = new List<string>();
        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            //Debug.Log($"GetScenesInBuild() path={path}");
            scenes.Add(System.IO.Path.GetFileNameWithoutExtension(path));
            //Debug.Log($"GetScenesInBuild() scenes[i]={scenes[i]}");
        }
        //Debug.Log($"GetScenesInBuild() sceneCount={sceneCount}");

        return scenes;
    }

    public int GetSceneIndexFromName(string _name) => sceneList.IndexOf(_name);
    public string GetSceneNameFromIndex(int _index) => sceneList[_index];
}


/// <summary>
/// "Pagination" struct with prev/current/next
/// ORIGINAL from Sneakaway Utilities
/// </summary>
[System.Serializable]
public class SceneControl_Indexer
{
    public int prev;
    public int current;
    public int next;
    public int count;
    // set default values
    public void Reset(int _count)
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