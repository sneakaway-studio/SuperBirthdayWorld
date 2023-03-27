using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 *  SceneControllerData - Makes scene data and functions globally available
 *  1. Create a "_ManagerScene" and make it the first in the Build Settings
 *  2. Attach SceneController.cs to GameObject in _ManagerScene
 *  3. Attach SceneControllerLoader.cs to a GameObject in each scene to load _ManagerScene
 */
public static class SceneControllerData
{
    // currently loaded scenes
    public static List<string> scenesLoaded;
    // current active scene
    public static string activeScene;
    // active scene (build) index
    public static int activeSceneIndex;
    // list of scenes from build index
    public static List<string> sceneList;
    // to track prev, current, next index
    public static Indexer sceneIndexer;
    // prefix and suffix to remove from paths
    static string scenePathPrefix = "Assets/Scenes/";
    static string scenePathSuffix = ".unity";



    public static void Init()
    {
        // get sceneList from build settings
        sceneList = GetAllScenesInBuild();

        // confirm if needed
        //if (!SceneControllerData.IfSceneListMatchesBuildIndex(_sceneList))
        //    Debug.LogWarning("Check scene list!!");

        // create indexer
        sceneIndexer = new Indexer(sceneList.Count);
        // get information about active scenes
        UpdateLoadedScenesData();
    }

    public static void UpdateLoadedScenesData()
    {
        scenesLoaded = GetScenesLoaded();
        activeScene = GetActiveSceneName();
        activeSceneIndex = GetBuildIndexFromSceneName(activeScene);
        // update current (in case we are starting on another scene)
        sceneIndexer.UpdateIndexes(activeSceneIndex);
    }



    ////////////////////////////////////////////////////// 
    ////////////////////// LOADERS /////////////////////// 
    ////////////////////////////////////////////////////// 

    /// <summary>
    /// Load the _ManagerScene (first scene in the build index)
    /// </summary>
    public static void CheckLoadSceneManager()
    {
        if (!IsSceneLoaded(0))
            SceneManager.LoadScene(0, LoadSceneMode.Additive);
    }
    /// <summary>
    /// Load previous scene by index
    /// </summary>
    public static void LoadPrevScene()
    {
        Debug.Log("LoadPrevScene() _sceneIndexer.prev=" + sceneIndexer.prev);
        if (!IsSceneLoaded(sceneIndexer.prev))
            SceneManager.LoadScene(sceneIndexer.prev, LoadSceneMode.Additive);
    }
    /// <summary>
    /// Load next scene by index
    /// </summary>
    public static void LoadNextScene()
    {
        Debug.Log("LoadNextScene() _sceneIndexer.next=" + sceneIndexer.next);
        if (!IsSceneLoaded(sceneIndexer.next))
            SceneManager.LoadScene(sceneIndexer.next, LoadSceneMode.Additive);
    }
    /// <summary>
    /// Loads scene by name
    /// </summary>
    /// <param name="sceneName"></param>
    public static void LoadScene(string sceneName)
    {
        if (!IsSceneLoaded(sceneName))
        {
            // save current
            int activeSceneBuildIndex = sceneIndexer.current;
            // load next
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            // unload previous active
            SceneManager.UnloadSceneAsync(activeSceneBuildIndex);
        }
    }


    /// <summary>
    /// Load a scene by name (use for gameManagers or any additive scene). Other methods attempted (see the previous commit for code):
    /// - Series of coroutines (issues calling them in static methods)
    /// - SceneHelper from https://gist.github.com/kurtdekker/862da3bc22ee13aff61a7606ece6fdd3
    /// </summary>
    public static async Task<bool> LoadSceneAsync(string _name, bool _additive, bool _setActive)
    {
        // make sure it isn't empty or null
        //if (!IsValidSceneName(_name)) return false;

        //// is it the same scene?
        //if (IsSceneLoaded(_name))
        //{
        //    Debug.LogWarning($"AdditiveSceneManager.LoadSceneAsync('{_name}') => ALREADY LOADED");
        //    //ReloadCurrentSceneAsync(); // just call it the right way, don't use this
        //    return false;
        //}

        // start loading scene
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_name, _additive ? LoadSceneMode.Additive : 0);
        int safety = 0;
        // wait until the scene is fully unloaded
        while (!asyncOperation.isDone)
        {
            // check if done 
            Debug.Log($"AdditiveSceneManager.LoadSceneAsync('{_name}') safety={safety} // progress={asyncOperation.progress}");
            if (++safety > 100)
            {
                Debug.LogError($"AdditiveSceneManager.LoadSceneAsync('{_name}') SAFETY FIRST!!!");
                break;
            }
            await Task.Delay(10); // milliseconds  
        }
        if (asyncOperation.isDone)
        {
            // complete loading
            if (_setActive) SetSceneActive(_name);
            //HideLoadingPanel();
            //Debug.Log($"AdditiveSceneManager.LoadSceneAsync('{_name}') [3.0] -> asyncOperation.isDone={asyncOperation.isDone}".Peach4());
            return true;
        }
        //else
        //Debug.LogError($"AdditiveSceneManager.LoadSceneAsync('{_name}') [3.1] -> COULD NOT LOAD THE SCENE".Red());
        return false;
    }
    public static async void SetSceneActive(string _name)
    {
        // wait for a frame to load to mark it active 
        await Task.Delay(10); // milliseconds  
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_name));
    }

    /// <summary>
    /// Loads scene by build index
    /// </summary>
    /// <param name="sceneName"></param>
    public static async Task LoadScene(int sceneIndex)
    {
        if (!IsSceneLoaded(sceneIndex))
        {
            await LoadSceneAsync(SceneManager.GetSceneByBuildIndex(sceneIndex).name, true, true);

            // save current
            int activeSceneBuildIndex = sceneIndexer.current;
            // load next
            //SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
            // unload previous active
            await UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(activeSceneBuildIndex).name);
        }
    }
    /// <summary>
    /// Unload a scene
    /// </summary>
    public static async Task<bool> UnloadSceneAsync(string _name = "")
    {
        Debug.Log($"AdditiveSceneManager.UnloadSceneAsync({_name}) [0]");

        // make sure it isn't empty or null
        //if (!IsValidSceneName(_name)) return false;
        // never unload the manager scene
        //if (IsManagerScene(_name)) return false;

        Debug.Log($"AdditiveSceneManager.UnloadSceneAsync({_name}) [1]");

        // start loading scene
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(_name);
        int safety = 0;
        // wait until the scene is fully unloaded
        while (!asyncOperation.isDone)
        {
            // check if done 
            Debug.Log($"AdditiveSceneManager.UnloadSceneAsync({_name}) [2] safety={safety} // progress={asyncOperation.progress}");
            if (++safety > 100)
            {
                Debug.LogError($"AdditiveSceneManager.UnloadSceneAsync({_name}) SAFETY FIRST!!!");
                break;
            }
            await Task.Delay(5); // milliseconds  
        }
        if (asyncOperation.isDone)
        {
            Debug.Log($"AdditiveSceneManager.UnloadSceneAsync('{_name}') [3.0] -> asyncOperation.isDone={asyncOperation.isDone}");
            return true;
        }
        //else
        Debug.LogError($"AdditiveSceneManager.UnloadSceneAsync({_name}) [3.1] -> COULD NOT UNLOAD THE SCENE");
        return false;
    }



    ////////////////////////////////////////////////////// 
    //////////////////////// DATA //////////////////////// 
    ////////////////////////////////////////////////////// 

    public static List<string> GetAllScenesInBuild()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        List<string> scenes = new List<string>();

        for (int i = 0; i < sceneCount; i++)
        {
            string str = SceneUtility.GetScenePathByBuildIndex(i)
                .Replace(scenePathPrefix, "")
                .Replace(scenePathSuffix, "");
            scenes.Add(str);
            //Debug.Log(str);
        }
        return scenes;
    }

    public static string GetActiveSceneName()
    {
        Scene scene = SceneManager.GetActiveScene();
        //Debug.Log("Active Scene name is: " + scene.name + "\nActive Scene index: " + scene.buildIndex);
        return scene.name;
    }
    public static int GetBuildIndexFromSceneName(string sceneName)
    {
        return SceneUtility.GetBuildIndexByScenePath(scenePathPrefix + sceneName + scenePathSuffix);
    }
    public static int GetScenesLoadedCount()
    {
        return SceneManager.sceneCount;
    }
    public static List<string> GetScenesLoaded()
    {
        List<string> loadedScenes = new List<string>();
        for (int i = 0; i < GetScenesLoadedCount(); i++)
        {
            loadedScenes.Add(SceneManager.GetSceneAt(i).name);
        }
        return loadedScenes;
    }
    /// <summary>
    /// Returns true if the scene is loaded AND valid
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    public static bool IsSceneLoaded(string _name)
    {
        Scene scene = SceneManager.GetSceneByName(_name);
        Debug.Log($"SceneFunctions.IsSceneLoaded('{_name}') scene.isLoaded={scene.isLoaded}, scene.IsValid()={scene.IsValid()}");
        return (scene.isLoaded && scene.IsValid());
    }
    // ^ overload
    public static bool IsSceneLoaded(int _index)
    {
        Scene scene = SceneManager.GetSceneByBuildIndex(_index);
        Debug.Log($"SceneFunctions.IsSceneLoaded('{_index}') scene.isLoaded={scene.isLoaded}, scene.IsValid()={scene.IsValid()}");
        return (scene.isLoaded && scene.IsValid());
    }



    ////////////////////////////////////////////////////// 
    /////////////////////// CHECKS /////////////////////// 
    ////////////////////////////////////////////////////// 


    /// <summary>
    /// Compare the scene list to the build index
    /// </summary>
    /// <param name="sceneList"></param>
    /// <returns></returns>
    public static bool IfSceneListMatchesBuildIndex(List<string> sceneList)
    {
        if (SceneManager.sceneCountInBuildSettings != sceneList.Count)
        {
            Debug.LogWarning($"Length of buildIndex ({SceneManager.sceneCountInBuildSettings}) and sceneList ({sceneList.Count}) don't match!");
            return false;
        }
        for (int i = 0; i < sceneList.Count; i++)
        {
            string buildScenePath = SceneUtility.GetScenePathByBuildIndex(i);

            if (buildScenePath != scenePathPrefix + sceneList[i] + scenePathSuffix)
            {
                Debug.LogWarning(buildScenePath + " != " + scenePathPrefix + sceneList[i] + scenePathSuffix);
                return false;
            }
        }
        return true;
    }





    ////////////////////////////////////////////////////// 
    /////////////////////// TYPES //////////////////////// 
    ////////////////////////////////////////////////////// 


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
