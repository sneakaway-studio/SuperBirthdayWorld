using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class to get, store, and use JSON file of scenes in build index
/// </summary>
[System.Serializable]
public class EasyScene_Data
{
    // path to save the data file
    public static string filePath = "Assets/Plugins/EasyScene/EasyScene_DataFile.json";

    private static EasyScene_SceneList _sceneList = new EasyScene_SceneList();
    [SerializeField]
    public static EasyScene_SceneList SceneList
    {
        get
        {   // try JSON first
            //if (_sceneList == null || _sceneList.names.Count <= 0)
            //{
            //    _sceneList = GetFromJson();
            //    Debug.Log($"EasyScene_Data.SceneList.names[2] = {_sceneList.names[2]}");
            //}
            //if it doesn't exist then save the json file
            if (_sceneList == null || _sceneList.names.Count <= 0)
            {
                _sceneList = new EasyScene_SceneList();
                _sceneList.names = GetScenesInBuild();
                Debug.Log($"EasyScene_Data.SceneList.names[2] = {_sceneList.names[2]}");
            }
            return _sceneList;
        }
    }

    /// <summary>Get the scene build list and save as JSON</summary>
    /// <param name="_path"></param>
    /// <param name="_name"></param>
    public static void SaveToJson(string _filePath = "")
    {
        if (filePath != "") filePath = _filePath;
        _sceneList.names = GetScenesInBuild();
        string data = JsonUtility.ToJson(_sceneList);
        System.IO.File.WriteAllText(filePath, data);
    }

    /// <summary>Return a list of all scenes in build</summary>
    /// <returns>List<string></returns>
    private static List<string> GetScenesInBuild()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        List<string> scenes = new List<string>();
        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            scenes.Add(System.IO.Path.GetFileNameWithoutExtension(path));
        }
        return scenes;
    }

    /// <summary>Return a custom object from a JSON file</summary>
    /// <returns>EasyScene_SceneList</returns>
    private static EasyScene_SceneList GetFromJson()
    {
        string str = System.IO.File.ReadAllText(filePath);
        Debug.Log(str);
        return JsonUtility.FromJson<EasyScene_SceneList>(str);
    }

    /// <summary>Get the index of a scene in the build list</summary>
    /// <param name="_name"></param>
    /// <returns>-1 if not found</returns>
    public static int GetSceneIndexFromName(string _name)
    {
        return SceneList.names.IndexOf(_name);
    }

    /// <summary>Get the name of a scene in the build list</summary>
    /// <param name="_index"></param>
    /// <returns>"" if not found</returns>
    public static string GetSceneNameFromIndex(int _index)
    {
        return SceneList.names[_index];
    }
}


////////////////////////////////////////////////////// 
/////////////////////// TYPES //////////////////////// 
////////////////////////////////////////////////////// 


/// <summary>
/// Data class for exported scene list
/// </summary>
[System.Serializable]
public class EasyScene_SceneList
{
    // list of the names from the build index
    public List<string> names = new List<string>();
}

/// <summary>
/// "Pagination" struct with prev/current/next
/// ORIGINAL from Sneakaway Utilities
/// </summary>
[System.Serializable]
public static class EasyScene_Indexer
{
    public static int prev;
    public static int current;
    public static int next;
    public static int count;
    // set default values
    public static void Reset(int _count)
    {
        count = _count; // e.g. count = 10 = 10,0,1
        prev = count - 1;
        current = 0;
        next = 1;
    }
    // advance to next | prev index, update values
    public static void NextIndex() => UpdateIndexes(next);
    public static void PrevIndex() => UpdateIndexes(prev);

    // call after current has been set, to set / check values
    public static void UpdateIndexes(int _current)
    {
        current = _current; // set current 
        if (current >= count) current = 0; // if should loop
        next = current + 1; // set next
        if (next >= count) next = 0; // if should loop
        prev = current - 1; // set prev
        if (prev < 0) prev = count - 1; // if should loop
    }
}

