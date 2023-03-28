using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEditor;

/**
 *  1. Load Manager scene
 *  2. Play scene
 */

public class EasyScene_Scene : MonoBehaviour
{
    [Tooltip("Is the manager already loaded")]
    public bool managerSceneLoaded;

    [Tooltip("Name of the scene this gameObject is in")]
    public string sceneName;

    [Tooltip("Build index for this scene")]
    public int sceneBuildIndex;

    [Tooltip("This scene is ready to play")]
    public bool sceneReady;

    [Tooltip("Whether to start the game from scratch on play (Editor)")]
    public string loadInitSceneOnPlay;

    // Editor only function
    void OnValidate()
    {
        // Only run in editor, and only in edit mode
        if (Application.isPlaying) return;

        //if (!managerSceneLoaded)
        //    Debug.LogWarning("MANAGER SCENE NOT FOUND => DRAG IT TO HIERARCHY");

        GetSceneData();

        // save the json file
        EasyScene_Data.SaveToJson("Assets/Plugins/EasyScene/EasyScene_DataFile.json");
    }

    void GetSceneData()
    {
        // get data about this scene
        managerSceneLoaded = EasyScene_Static.IsSceneLoaded("ManagerScene");
        sceneName = gameObject.scene.name;
        sceneBuildIndex = gameObject.scene.buildIndex;
    }

    void Awake() => GetSceneData();

    void Start()
    {
        managerSceneLoaded = EasyScene_Static.IsSceneLoaded("ManagerScene");
        // check and load manager
        if (!managerSceneLoaded) EasyScene_Static.LoadManagerScene();
    }


}
