using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyScene_Loader : MonoBehaviour
{
    public enum OnEvent { Awake, Enable, Start, Collision, Trigger, Time };
    public OnEvent onEvent;

    public enum LoadScene { previous, current, next, first, last, manager, name };
    public LoadScene sceneToLoad;

    [SerializeField] string sceneName;
    [SerializeField] bool loadAsAdditive = true;
    [SerializeField] bool triggered;

    async void Load(OnEvent _onEvent)
    {
        if (onEvent != _onEvent) return;

        Debug.Log($"+++ 0 +++ EasyScene_Loader() onEvent = {onEvent}, sceneToLoad = {sceneToLoad}");
        if (triggered) return;
        triggered = true;
        switch (sceneToLoad)
        {
            case LoadScene.previous:
                EasyScene_Static.LoadPreviousScene();
                break;
            case LoadScene.current:
                EasyScene_Static.LoadCurrentScene();
                break;
            case LoadScene.next:
                EasyScene_Static.LoadNextScene();
                break;
            case LoadScene.first:
                EasyScene_Static.LoadFirstScene();
                break;
            case LoadScene.last:
                EasyScene_Static.LoadLastScene();
                break;
            case LoadScene.manager:
                EasyScene_Static.LoadManagerScene();
                break;
            case LoadScene.name:
                if (sceneName != "") await EasyScene_Static.LoadSceneAsync(sceneName, loadAsAdditive);
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
