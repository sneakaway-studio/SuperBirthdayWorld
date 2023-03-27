using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 *  SceneControllerManager - Displays SceneControllerData (see for notes)
 */
public class SceneController : MonoBehaviour
{
    // these are all local copies for display only

    [SerializeField] List<string> _scenesLoaded;
    [SerializeField] string _activeScene;
    [SerializeField] int _activeSceneIndex;
    [SerializeField] List<string> _sceneList = new List<string>();
    [SerializeField] SceneControllerData.Indexer _sceneIndexer;

    private void OnValidate()
    {
        SceneControllerData.Init();
        _sceneList = SceneControllerData.sceneList;
        _sceneIndexer = SceneControllerData.sceneIndexer;
        UpateData();
    }

    void LateUpdate() => UpateData();

    void UpateData()
    {
        SceneControllerData.UpdateLoadedScenesData();
        _scenesLoaded = SceneControllerData.GetScenesLoaded();
        _activeScene = SceneControllerData.GetActiveSceneName();
        _activeSceneIndex = SceneControllerData.GetBuildIndexFromSceneName(_activeScene);
        _sceneIndexer = SceneControllerData.sceneIndexer;
    }





}
