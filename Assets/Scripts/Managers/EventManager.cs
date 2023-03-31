using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

/** 
 *  EventManager class (modified to be fully static) originally from 
 *  https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events
 *   - If extreme performance need then modify to use Actions
 *      Code: https://stackoverflow.com/questions/42034245/unity-eventmanager-with-delegate-instead-of-unityevent
 *      Benchmarks: https://www.reddit.com/r/Unity3D/comments/35sa5h/unityevent_vs_delegate_event_benchmark_for_those
 *      Deeper: https://www.jacksondunstan.com/articles/3335
 *  - Add parameters to UnityEvent
 *      https://www.youtube.com/watch?v=kGykP7VZCvg
 *      https://stackoverflow.com/questions/42034245/unity-eventmanager-with-delegate-instead-of-unityevent
 */

public static class EventManager
{
    static bool DEBUG = false;

    // hold references to events
    private static Dictionary<string, UnityEvent> eventDictionary;
    // show event count
    public static int eventCount = 0;
    // show event names (as list because you can serialize)
    public static List<string> eventNames = new List<string>();
    // last caller
    public static string lastCallerName;


    // static constructor
    static EventManager()
    {
        if (eventDictionary == null)
            eventDictionary = new Dictionary<string, UnityEvent>();
    }

    /// <summary>Add event listener</summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public static void StartListening(string eventName, UnityAction listener)
    {
        if (eventName == "")
            if (DEBUG) Debug.Log($"EventManager.StartListening() [1] eventName from {listener.Target.ToString()} is empty");


        UnityEvent thisEvent = null;
        // is there already a key/value pair?
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            // add new event
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            eventDictionary.Add(eventName, thisEvent);
        }
        UpdateEventManagerInfo();
    }

    /// <summary>Remove listener from this event</summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public static void StopListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
        UpdateEventManagerInfo();
    }

    public static void TriggerEvent(string eventName, string callerName = "")
    {
        if (DEBUG) Debug.Log($"EventManager.TriggerEvent() [1] {callerName} => '{eventName}' (total: {eventDictionary.Count})");


        string logAllEvents = "";
        foreach (KeyValuePair<string, UnityEvent> e in eventDictionary)
        {
            logAllEvents += $"\n -> {e.Key} => { e.Value} ";
        }
        //if (DEBUG) Debug.Log($"EventManager.TriggerEvent() [2] ALL EVENTS: (total: {eventDictionary.Count}) {logAllEvents}".Pink());


        UnityEvent thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            if (DEBUG) Debug.Log($"EventManager.TriggerEvent() [3] {callerName} => '{eventName}' (total: {eventDictionary.Count})");
            // reset or save name of last calling gameObject
            lastCallerName = callerName;
            // invoke event 
            thisEvent.Invoke();
        }
        else
        {
            if (DEBUG) Debug.LogWarning($"EventManager.TriggerEvent() [4] {callerName} => '{eventName}' NOT FOUND");
        }
    }


    public static void UpdateEventManagerInfo()
    {
        // get count 
        eventCount = eventDictionary.Count;
        // clear list
        eventNames.Clear();
        // add names to list to display in inspector
        foreach (var e in eventDictionary)
        {
            eventNames.Add(e.Key);
        }
    }

}



