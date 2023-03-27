using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class EasyScene_Manager : MonoBehaviour
{
    [Header("Active Scene")]

    public Scene activeScene;
    public string activeSceneName;
    public int activeSceneIndex;
    public bool activeSceneLoaded;

    [Header("Scene Index")]

    public int count;
    public int prev;
    public int current;
    public int next;


    //[SerializeField]
    //public EasyScene_Indexer sceneIndex;

    ////////////////////////////////////////////////////// 
    ///////////////////// LISTENERS //////////////////////
    //////////////////////////////////////////////////////

    private void OnValidate() => ResetIndexer();
    private void Awake() => ResetIndexer();

    void ResetIndexer()
    {
        EasyScene_Indexer.Reset(EasyScene_Data.SceneList.names.Count);
        UpdateDataAfterSceneLoad();
    }


    // OnEnable / OnDisable listeners
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    /// <summary>When any scene has loaded (notified using delegate)</summary>
    void OnSceneLoaded(Scene newScene, LoadSceneMode mode)
    {
        Debug.Log($"@@@ 0 @@@ OnSceneLoaded() newScene.name={newScene.name}, mode={mode}, activeSceneName={activeSceneName}");

        UpdateDataAfterSceneLoad();
    }

    /// <summary>When the active scene has changed (after OnSceneLoaded) (called from delegate)</summary>
    async void OnActiveSceneChanged(Scene previousActive, Scene newActive)
    {
        Debug.Log($"@@@ 1 @@@ OnActiveSceneChanged() previousActive.name='{previousActive.name}', newActive.name={newActive.name}");

        // unload the previously active scene (method includes checks)
        await EasyScene_Static.UnloadSceneAsync(previousActive.name);

        UpdateDataAfterSceneLoad();
    }

    /// <summary>
    /// When any scene has unloaded (notified using delegate)
    /// </summary>
    void OnSceneUnloaded(Scene current)
    {
        Debug.Log($"@@@ 2 @@@ OnSceneUnloaded() current.name='{current.name}', activeSceneName={activeSceneName}");

        UpdateDataAfterSceneLoad();
    }

    /// <summary>Update details on active scene</summary>
    public void UpdateDataAfterSceneLoad(string caller = "")
    {
        // update active
        activeScene = SceneManager.GetActiveScene();
        activeSceneName = activeScene.name;
        activeSceneIndex = activeScene.buildIndex;
        activeSceneLoaded = activeScene.isLoaded;

        // update indexes
        EasyScene_Indexer.UpdateIndexes(activeSceneIndex);
        count = EasyScene_Indexer.count;
        prev = EasyScene_Indexer.prev;
        current = EasyScene_Indexer.current;
        next = EasyScene_Indexer.next;
    }




}
