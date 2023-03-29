using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Singleton (and children "Service Locator pattern)
 *  References
 *  https://gamedevbeginner.com/singletons-in-unity-the-right-way/
 *  https://gamedev.stackexchange.com/a/116019/60431
 */

public class Singleton : MonoBehaviour
{
    // Singleton is accessible (public static)
    public static Singleton Instance { get; private set; }
    public bool created = false;

    // References to others ("Service Locator" pattern)
    public SceneControl SceneControl { get; set; } // non-private set
    public MusicManager MusicManager { get; set; }
    //public UIManager UIManager { get; private set; }

    private void Awake()
    {
        if (Singleton.Instance != null && Singleton.Instance.created)
        {
            Debug.Log("Another Singleton already exists!!!!");
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            DestroyImmediate(this.gameObject);
            return;
        }
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        created = true;
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        // store references
        SceneControl = GetComponent<SceneControl>();
        MusicManager = GetComponentInChildren<MusicManager>();
        //UIManager = GetComponentInChildren<UIManager>();
    }
}