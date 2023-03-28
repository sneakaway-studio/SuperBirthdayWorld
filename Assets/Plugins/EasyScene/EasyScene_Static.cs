using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public static class EasyScene_Static
{
    /**
     * 
     *  INSTRUCTIONS
     * 
     *  1. Add scenes in build to be accessible by this class. Order should be:
     *      1. Scenes/_Initialization
     *      2. Scenes/ManagerScene
     *      3. ... <all other scenes>
     *  2. Add a GameObject to each scene
     *  
     *  
     */

    static bool log = true;




    ////////////////////////////////////////////////////// 
    //////////////////// LOAD SCENES /////////////////////
    //////////////////////////////////////////////////////

    /// <summary>Load manager scene</summary>
    public static async void LoadManagerScene()
    {
        if (!IsSceneLoaded("ManagerScene"))
        {
            Debug.Log("!!! 1 !!! LOADING MANAGER SCENE");
            await LoadSceneAsync(_name: "ManagerScene", _additive: true, _setActive: false);
        }
    }
    /// <summary>Load previous scene</summary>
    public static async void LoadPreviousScene()
    {
        await LoadSceneAsync(_name: EasyScene_Data.GetSceneNameFromIndex(EasyScene_Indexer.prev), _additive: true);
    }
    /// <summary>Load (reload) current scene</summary>
    public static async void LoadCurrentScene()
    {
        await LoadSceneAsync(_name: EasyScene_Data.GetSceneNameFromIndex(EasyScene_Indexer.current), _additive: false, _reload: true);
    }
    /// <summary>Load next scene</summary>
    public static async void LoadNextScene()
    {
        Debug.Log($"EasyScene_Static.LoadNextScene() index={EasyScene_Indexer.next} name={EasyScene_Data.GetSceneNameFromIndex(EasyScene_Indexer.next)}");
        await LoadSceneAsync(_name: EasyScene_Data.GetSceneNameFromIndex(EasyScene_Indexer.next), _additive: true);
    }
    /// <summary>Load first scene</summary>
    public static async void LoadFirstScene()
    {
        await LoadSceneAsync(_name: EasyScene_Data.GetSceneNameFromIndex(0), _additive: true);
    }
    /// <summary>Load last scene</summary>
    public static async void LoadLastScene()
    {
        await LoadSceneAsync(_name: EasyScene_Data.GetSceneNameFromIndex(EasyScene_Indexer.count - 1), _additive: true);
    }



    /// <summary>
    /// Load a scene by name (use for gameManagers or any multi scene)
    /// </summary>
    public static async Task<bool> LoadSceneAsync(string _name, bool _additive, bool _setActive = true, bool _reload = false)
    {
        if (log) Debug.Log($"+++ 0 +++ LoadSceneAsync('{_name}'), _additive={_additive}, _setActive={_setActive}");

        // make sure it isn't empty or null
        if (!IsValidSceneName(_name)) return false;
        // quit if the same scene    
        if (IsSceneLoaded(_name) && !_reload)
        {
            Debug.LogWarning($"+++ X +++ LoadSceneAsync('{_name}') => ALREADY LOADED");
            return false;
        }
        // start loading scene
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_name, _additive ? LoadSceneMode.Additive : 0);

        int safety = 0;
        // wait until the scene is fully loaded
        while (!asyncOperation.isDone)
        {
            // check if done 
            if (log) Debug.Log($"+++ 1.{safety} +++ LoadSceneAsync('{_name}'), safety={safety}, progress={asyncOperation.progress}");
            if (++safety > 100)
            {
                Debug.LogError($"+++ X +++ LoadSceneAsync('{_name}') SAFETY FIRST!!!");
                break;
            }
            await Task.Delay(1); // milliseconds  
        }
        // complete loading
        if (asyncOperation.isDone)
        {
            if (log) Debug.Log($"+++ 2 +++ LoadSceneAsync('{_name}') asyncOperation.isDone={asyncOperation.isDone}");

            //await Task.Delay(10); // wait for a frame to load to mark it active

            // now that everything is loaded, make sure this is the active scene
            if (_setActive) SetSceneActive(_name);

            //HideLoadingPanel(); // if using this
            return true;
        }
        //else
        Debug.LogError($"+++ X +++ LoadSceneAsync('{_name}') COULD NOT LOAD THE SCENE");
        return false;
    }

    /// <summary>Set a scene active (by name or scene)</summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    public static bool SetSceneActive(string _name) => SetSceneActive(SceneManager.GetSceneByName(_name));
    public static bool SetSceneActive(Scene _scene)
    {
        if (log) Debug.Log($"*** 0 *** SetSceneActive('{_scene.name}') SceneManager.GetActiveScene().name {SceneManager.GetActiveScene().name}");
        if (SceneManager.SetActiveScene(_scene))
        {
            if (log) Debug.Log($"*** 1 *** SetSceneActive('{_scene.name}') SceneManager.GetActiveScene().name {SceneManager.GetActiveScene().name}");
            return true;
        }
        Debug.LogWarning($"*** X *** SetSceneActive('{_scene.name}') SceneManager.GetActiveScene().name {SceneManager.GetActiveScene().name} COULD NOT SET {_scene.name} SCENE ACTIVE");
        return false;
    }

    /// <summary>Unload a scene (by name) async</summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    public static async Task<bool> UnloadSceneAsync(string _name = "")
    {
        if (log) Debug.Log($"--- 0 --- UnloadSceneAsync({_name}) [0]");

        // make sure it isn't empty or null
        if (!IsValidSceneName(_name)) return false;
        // never unload the manager scene
        if (IsManagerScene(_name)) return false;

        // start unloading scene
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(_name);
        int safety = 0;
        // wait until the scene is fully unloaded
        while (!asyncOperation.isDone)
        {
            // check if done 
            if (log) Debug.Log($"--- 1.{safety} --- UnloadSceneAsync({_name}) safety={safety}, progress={asyncOperation.progress}");
            if (++safety > 100)
            {
                Debug.LogError($"--- X --- UnloadSceneAsync({_name}) SAFETY FIRST!!!");
                break;
            }
            await Task.Delay(5); // milliseconds  
        }
        if (asyncOperation.isDone)
        {
            if (log) Debug.Log($"--- 2 --- UnloadSceneAsync('{_name}') asyncOperation.isDone={asyncOperation.isDone}");
            return true;
        }
        //else
        Debug.LogError($"--- X --- UnloadSceneAsync({_name}) COULD NOT UNLOAD THE SCENE");
        return false;
    }




    ////////////////////////////////////////////////////// 
    /////////////////// SCENE STATUS /////////////////////
    //////////////////////////////////////////////////////

    /// <summary>Returns true if the scene is loaded AND valid</summary>
    public static bool IsSceneLoaded(string _name)
    {
        Scene scene = SceneManager.GetSceneByName(_name);
        if (log) Debug.Log($"??? 0 ??? IsSceneLoaded('{_name}') scene.isLoaded={scene.isLoaded}, scene.IsValid()={scene.IsValid()}");
        return scene.isLoaded && scene.IsValid();
    }
    /// <summary>Returns true if valid name</summary>
    public static bool IsValidSceneName(string _name)
    {
        if (_name == null || _name == "")
        {
            Debug.LogWarning($"??? 0 ??? IsValidSceneName('{_name}') _name is NULL || EMPTY => NOT A VALID SCENE NAME");
            return false;
        }
        return true;
    }
    /// <summary>Returns true if _name is the manager scene</summary>
    public static bool IsManagerScene(string _name)
    {
        if (_name.ToLower().Contains("manager"))
        {
            Debug.LogWarning($"??? 0 ??? IsManagerScene('{_name}') => DO NOT UNLOAD MANAGER SCENE!");
            return true;
        }
        // else
        return false;
    }

}


