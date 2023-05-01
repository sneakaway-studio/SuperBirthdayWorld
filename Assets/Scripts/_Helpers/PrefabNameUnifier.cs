using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

// Auto-rename Prefab instances (in Scene) when you change it (in Project)
// https://answers.unity.com/questions/654151/changing-a-prefabs-name-via-the-editor-doesnt-appl.html

[ExecuteInEditMode]
public class PrefabNameUnifier : MonoBehaviour
{
    [SerializeField] GameObject prefab;

#if UNITY_EDITOR

    void OnEnable()
    {
        if (InPrefabIsolationMode()) return;
        if (Application.isPlaying) return; 

        if (PrefabUtility.GetPrefabAssetType(gameObject) == PrefabAssetType.Regular || PrefabUtility.GetPrefabAssetType(gameObject) == PrefabAssetType.Variant)
        {
            prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject) as GameObject;
            EditorApplication.update += CheckForNameChange;
        }
    }
    void OnDisable()
    {
        EditorApplication.update -= CheckForNameChange;
    }
#endif

    void CheckForNameChange()
    {
        if (prefab.name != name)
        {
            print("Changing instance name from: " + name + " to " + prefab.name);
            name = prefab.name;
        }
    }

    bool InPrefabIsolationMode()
    {
        return EditorSceneManager.IsPreviewScene(SceneManager.GetActiveScene());
    }

}