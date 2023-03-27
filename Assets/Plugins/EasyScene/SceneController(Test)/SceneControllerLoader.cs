using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  SceneControllerLoader
 *  - Place in each "child" scene to ensure SceneController is loaded
 */

public class SceneControllerLoader : MonoBehaviour
{
    private void Start()
    {
        SceneControllerData.CheckLoadSceneManager();
    }
}
