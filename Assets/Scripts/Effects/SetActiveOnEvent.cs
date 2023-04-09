using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveOnEvent : MonoBehaviour
{
    public enum OnEvent { Awake, Enable, Start, Collision, Trigger, Time };
    [Tooltip("Event to act on")]
    public OnEvent onEvent;

    [Tooltip("Component to enable / disable")]
    public Behaviour component;
    public SpriteRenderer spriteRenderer;

    [Tooltip("State to set")]
    public bool enable = true;

    [Tooltip("Has it been triggered?")]
    public bool triggered;

    void SetEnabled(OnEvent _onEvent)
    {
        if (onEvent == _onEvent && !triggered)
        {
            if (component != null)
                component.enabled = enable;
            if (spriteRenderer != null)
                spriteRenderer.GetComponent<Renderer>().enabled = enable;
            triggered = true;
        }
    }

    // EVENTS

    void Awake() => SetEnabled(OnEvent.Awake);
    void OnEnable() => SetEnabled(OnEvent.Enable);
    void Start() => SetEnabled(OnEvent.Start);

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.transform.tag);
        // NOTE: If using tags for collision checking etc. only add the tag to one GameObject in a scene.
        // Do not add the tag to its children as well, or you will be getting references to the wrong gameobjects!
        if (collision.transform.parent.tag == "Player") SetEnabled(OnEvent.Collision);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.transform.tag);
        if (collision.transform.parent.tag == "Player") SetEnabled(OnEvent.Trigger);
    }


}
