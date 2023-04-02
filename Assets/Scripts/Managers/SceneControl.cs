using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

/**
 *  SceneControl ("Singleton" + "Service Locator" patterns)
 *  2023 Owen Mundy 
 *  References
 *  https://gamedevbeginner.com/singletons-in-unity-the-right-way/
 *  https://gamedev.stackexchange.com/a/116019/60431
 */

public class SceneControl : MonoBehaviour
{
    // *** SINGLETON => make instance accessible outside of class
    public static SceneControl Instance { get; private set; }
    // *** SINGLETON => only create once
    public bool singletonCreated = false;


    [Header("Spawn Points")]

    public Transform player;
    public Transform enterPoint;
    public Transform exitPoint;

    [Header("Scene Data")]

    [Tooltip("Number of scenes in the build")]
    public int sceneCount;
    [Tooltip("List of scene names in the build")]
    public List<string> sceneList;
    [Tooltip("Index of prev / current / next scenes")]
    public SceneControl_Indexer indexer;

    [Header("Current Scene")]

    [Tooltip("Active scene")]
    Scene activeScene;
    [Tooltip("Active scene is loaded")]
    public bool activeSceneLoaded;
    [Tooltip("Active scene name")]
    public string activeSceneName;
    [Tooltip("Active scene index")]
    public int activeSceneIndex;
    [Tooltip("Active scene level")]
    public int activeSceneLevel;
    [Tooltip("Active scene number")]
    public string activeSceneNumber;



    [Header("Previous Scene")]

    [Tooltip("Previous scene index")]
    public int previousSceneIndex;
    [Tooltip("Previous scene level")]
    public int previousSceneLevel;


    [Tooltip("Index of prev / current / next scenes")]
    public TMP_Text sceneDebugText;


    // @@@ SERVICE LOCATOR => References to class instances on child objects
    public MusicManager MusicManager { get; set; } // non-private set
    //public UIManager UIManager { get; private set; }

    private void Awake()
    {
        // *** SINGLETON => If instance exists ...
        if (SceneControl.Instance != null && SceneControl.Instance.singletonCreated)
        {
            //Debug.Log("Another SceneControl (Singleton) already exists; deleting child objects...");
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            // *** SINGLETON => Then delete the object and exit
            DestroyImmediate(this.gameObject);
            return;
        }
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        // *** SINGLETON => Only reach this point on the first load...
        Debug.Log($"*** SceneControl (Singleton) created");
        singletonCreated = true;
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        // @@@ SERVICE LOCATOR => Store references 
        MusicManager = GetComponentInChildren<MusicManager>();
        //UIManager = GetComponentInChildren<UIManager>();

        // create scene list
        if (sceneList == null || sceneList.Count < 1) CreateSceneList();
    }

    void CreateSceneList()
    {
        Debug.Log($"^^^ 0 ^^^ SceneControl.CreateSceneList()");
        // if the sceneList has not been created
        sceneCount = SceneManager.sceneCountInBuildSettings;
        indexer = new SceneControl_Indexer();
        indexer.Reset(sceneCount);
        sceneList = GetScenesInBuild();
        UpdateActiveSceneData();
    }

    void UpdateActiveSceneData()
    {
        Debug.Log($"^^^ 1 ^^^ SceneControl.UpdateActiveSceneData()");
        previousSceneIndex = activeSceneIndex;
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

    /// <summary>When the active scene has changed (after OnSceneLoaded) (called from delegate)</summary>
    //void OnActiveSceneChanged(Scene previousActive, Scene newActive)
    //Debug.Log($"^^^ 2 ^^^  SceneControl.OnActiveSceneChanged() previousActive.name='{previousActive.name}', newActive.name={newActive.name}");
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        Debug.Log($"^^^ 2 ^^^  SceneControl.OnActiveSceneChanged() previousSceneIndex={previousSceneIndex} > scene.buildIndex={scene.buildIndex} scene.name={scene.name}");

        // determines which door to move the player
        //previousSceneIndex = previousActive.buildIndex;

        UpdateActiveSceneData();

        // determines which music theme to start
        activeSceneNumber = scene.name.Replace("Scene-", "");
        string[] words = activeSceneNumber.Split("-", System.StringSplitOptions.RemoveEmptyEntries);
        int newActiveSceneLevel = Int32.Parse(words[0]);
        if (newActiveSceneLevel != activeSceneLevel)
        {
            activeSceneLevel = newActiveSceneLevel;
            EventManager.TriggerEvent("UpdateTheme");
        }


        // update player references
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enterPoint = GameObject.Find("EnterPoint").transform;
        exitPoint = GameObject.Find("ExitPoint").transform;
        // move the player into the correct position        
        if (previousSceneIndex <= activeSceneIndex)
        {
            player.position = enterPoint.position;
        }
        else
        {
            player.position = exitPoint.position;
            // face the character to the left
            player.GetComponent<PlayerControl3>().Flip();
        }

    }



    ////////////////////////////////////////////////////// 
    ///////////////////// LISTENERS //////////////////////
    //////////////////////////////////////////////////////


    // OnEnable / OnDisable listeners
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //SceneManager.activeSceneChanged += OnActiveSceneChanged;
        //SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        //SceneManager.activeSceneChanged -= OnActiveSceneChanged;
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
        previousSceneIndex = activeSceneIndex;
        previousSceneLevel = activeSceneLevel;

        // also could use async version?
        SceneManager.LoadScene(_name);
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