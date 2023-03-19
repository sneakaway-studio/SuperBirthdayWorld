using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string sceneToLoad;

    void Awake() => Init();

    private void Init()
    {
        SceneFunctions.LoadScene(sceneToLoad);
    }




}
