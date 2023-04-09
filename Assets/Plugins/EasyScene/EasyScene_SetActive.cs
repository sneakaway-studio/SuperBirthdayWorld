using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyScene_SetActive : MonoBehaviour
{
    public enum OnEvent { Awake, Enable, Start, Collision, Trigger, Time };
    public OnEvent onEvent;

    public Behaviour component;
    public bool enable = true;
    public bool triggered;

    void SetEnabled(OnEvent _onEvent)
    {
        if (onEvent == _onEvent && !triggered)
        {
            component.enabled = enable;
            triggered = true;
        }
    }

    void Awake() => SetEnabled(OnEvent.Awake);
    void OnEnable() => SetEnabled(OnEvent.Enable);
    void Start() => SetEnabled(OnEvent.Start);


}
