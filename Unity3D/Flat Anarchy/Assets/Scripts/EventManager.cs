using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> EventDictionary;

    private static EventManager eventManager;

    private static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
    ;    }    
    }

	// Use this for initialization
	private void Init()
    {
	    if (this.EventDictionary == null)
	    {
	        this.EventDictionary= new Dictionary<string, UnityEvent>();
	    }
	}
	
	public static void StartListening (string eventName, UnityAction listener)
	{
	    UnityEvent thisEvent = null;
	    if (instance.EventDictionary.TryGetValue(eventName, out thisEvent))
	    {
	        thisEvent.AddListener(listener);
	    }
	    else
	    {
	        thisEvent = new UnityEvent();
	        thisEvent.AddListener(listener);
	        instance.EventDictionary.Add(eventName, thisEvent);
	    }
	}

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (eventManager == null)
        {
            return;
        }

        UnityEvent thisEvent = null;
        if (instance.EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}
