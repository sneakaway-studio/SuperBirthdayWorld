using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Generic scene loader to load scene on (awake, collision, etc.) event
 *  2023 Owen Mundy
 */

public class SceneControl_Loader : MonoBehaviour
{
    public enum OnEvent { Awake, Enable, Start, Collision, Trigger, Time };
    public OnEvent onEvent;

    public enum LoadScene { previous, current, next, first, last, manager, name };
    public LoadScene sceneToLoad;

    [SerializeField] string sceneName;
    [SerializeField] bool triggered = false;

    void Load(OnEvent _onEvent)
    {
        if (onEvent != _onEvent) return;

        Debug.Log($"+++ 0 +++ SceneControl_Loader() onEvent = {onEvent}, sceneToLoad = {sceneToLoad}");
        if (triggered) return;
        triggered = true;
        switch (sceneToLoad)
        {
            case LoadScene.previous:
                SceneControl.Instance.LoadPreviousScene();
                break;
            case LoadScene.current:
                SceneControl.Instance.LoadCurrentScene();
                break;
            case LoadScene.next:
                SceneControl.Instance.LoadNextScene();
                break;
            case LoadScene.first:
                SceneControl.Instance.LoadFirstScene();
                break;
            case LoadScene.last:
                SceneControl.Instance.LoadLastScene();
                break;
            case LoadScene.name:
                if (sceneName != "") SceneControl.Instance.LoadScene(sceneName);
                break;
        }
    }

    void Awake() => Load(OnEvent.Awake);
    void OnEnable() => Load(OnEvent.Enable);
    void Start() => Load(OnEvent.Start);

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.transform.tag);
        if (collision.transform.tag == "Player") Load(OnEvent.Collision);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.transform.tag);
        if (collision.transform.tag == "Player") Load(OnEvent.Trigger);
    }



}
