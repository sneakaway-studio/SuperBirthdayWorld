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


    [Header("Scene-Specific")]

    public GameObject player;
    public Rigidbody2D playerRb;
    public Transform enterPoint;
    public Transform exitPoint;
    public GameObject currentBot;
    public string currentBotName;

    [Header("Scene Data")]

    [Tooltip("Number of scenes in the build")]
    public int sceneCount;
    [Tooltip("List of scene names in the build")]
    public List<string> sceneList;
    [Tooltip("Index of prev / current / next scenes")]
    public SceneControl_Indexer indexer;

    [Header("Current Scene")]

    [Tooltip("Active scene")] // Scene
    Scene activeScene;
    [Tooltip("Active scene is loaded")] // true
    public bool activeSceneLoaded;
    [Tooltip("Active scene name")] // "Scene-2-1"
    public string activeSceneName;
    [Tooltip("Active scene build index")] // 5
    public int activeSceneIndex;
    [Tooltip("Active scene level and number string")] // "2-1"
    public string activeSceneLevelString;
    [Tooltip("Active scene level")] // 2
    public int activeSceneLevel;
    [Tooltip("Active scene number in level")] // 1
    public int activeSceneNumberInLevel;
    [Tooltip("Menu, Home, Credits, Test, etc.")]
    public bool metaScene = false;

    [Header("Previous Scene")]

    [Tooltip("Previous scene index")]
    public int previousSceneIndex;
    [Tooltip("Previous scene level")]
    public int previousSceneLevel;


    [Tooltip("Index of prev / current / next scenes")]
    public TMP_Text sceneDebugText;
    public TeleType messageTextTeletyper;


    // @@@ SERVICE LOCATOR => References to class instances on child objects
    public MusicManager MusicManager { get; set; } // public set
    //public UIManager UIManager { get; private set; }

    private void Awake()
    {
        // *** SINGLETON => If instance exists ...
        if (Instance != null && Instance.singletonCreated)
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

        // default to zero (for test scenes)
        int newActiveSceneLevel = 0;
        activeSceneNumberInLevel = 0;

         metaScene = !scene.name.Contains("Scene-");


        // determines which music theme to start
        activeSceneLevelString = scene.name.Replace("Scene-", "");
        string[] level = activeSceneLevelString.Split("-", System.StringSplitOptions.RemoveEmptyEntries);

        // unless data found
        if (level.Length > 1)
        {
            Int32.TryParse(level[0], out newActiveSceneLevel);
            Int32.TryParse(level[1], out activeSceneNumberInLevel);
        }
        if (newActiveSceneLevel != activeSceneLevel)
        {
            activeSceneLevel = newActiveSceneLevel;
            EventManager.TriggerEvent("UpdateTheme");
        }


        ResetBotAndMessage();

        UpdateSceneReferences();

        if (scene.name.Contains("0-0"))
        {
            // home has different exit points for each level
            List<Transform> exitPoints = new List<Transform>();
            exitPoints.Add(GameObject.Find("ExitPoint1").transform);
            exitPoints.Add(GameObject.Find("ExitPoint2").transform);
            exitPoints.Add(GameObject.Find("ExitPoint3").transform);
            exitPoints.Add(GameObject.Find("ExitPoint4").transform);
            playerRb.position = exitPoints[previousSceneLevel].position;
            if (previousSceneLevel <= 2)
                // face the character to the left
                playerRb.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            // only one exit point on all levels
            exitPoint = GameObject.Find("ExitPoint").transform;
            // move the player into the correct position     
            if (previousSceneIndex <= activeSceneIndex)
            {
                playerRb.position = enterPoint.position;
            }
            else
            {
                playerRb.position = exitPoint.position;
                // face the character to the left
                playerRb.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }


    }


    void UpdateSceneReferences()
    {
        if (player == null)
        {
            // NOTE: If using tags for collision checking etc. only add the tag to one GameObject in a scene.
            // Do not add the tag to its children as well, or you will be getting references to the wrong gameobjects!
            //Debug.LogError("No player found; getting reference");
            player = GameObject.FindGameObjectWithTag("Player");
            //Debug.LogError(player);
        }
        if (playerRb == null)
        {
            //Debug.LogError("No player rb2d found; getting reference");
            playerRb = player.GetComponent<Rigidbody2D>();
            //Debug.LogError(playerRb);
        }
        if (enterPoint == null)
        {
            //Debug.LogError("No enterPoint found; getting reference");
            enterPoint = GameObject.Find("EnterPoint").transform;
            //Debug.LogError(enterPoint);
        }
    }


    private void Update()
    {
        // update player references
        UpdateSceneReferences();
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
        // save current active scene information
        previousSceneIndex = activeSceneIndex;
        previousSceneLevel = activeSceneLevel;
        // then load scene (also could use async version?)
        SceneManager.LoadScene(_name);
        // turn it back up
        MusicManager.TurnMusicUp();
    }




    ////////////////////////////////////////////////////// 
    /////////////////// BOT BUSINESS /////////////////////
    //////////////////////////////////////////////////////

    public void OnNewBotMessage(GameObject _bot)
    {
        if (currentBot != null)
        {
            currentBot.GetComponent<Bot>().OnEndMessageLocal();
        }
        currentBot = _bot;

        MusicManager.TurnMusicDown();
    }

    // called from bot or teletype
    public void OnEndBotMessage(GameObject _bot)
    {
        // if the same bot, then shut it all down
        if (currentBot == _bot)
        {
            ResetBotAndMessage();
            MusicManager.TurnMusicUp();
        }
        else
        {
            // ignore, let the new bot call this
        }
    }

    void ResetBotAndMessage()
    {
        currentBot = null;
        // hide teletype if left open
        SceneControl.Instance.messageTextTeletyper.OnCancelTyping();
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