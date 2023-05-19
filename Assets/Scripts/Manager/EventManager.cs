using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public static class EventManager
{
    private static Dictionary<string, List<Action<object[]>>> eventDictonary=new Dictionary<string, List<Action<object[]>>>();


    //Ìí¼ÓÊÂ¼ş¼àÌıÆ÷
    public static void AddListener(string eventName, Action<object[]> listener)
    {
        if (eventDictonary.TryGetValue(eventName, out List<Action<object[]>> listeners))
        {
            listeners.Add(listener);
        }
        else
        {
            listeners = new List<Action<object[]>>();
            listeners.Add(listener);
            eventDictonary.Add(eventName,listeners);
        }
    }

    public static void RemoveListener(string eventName, Action<object[]> listener)
    {
        if (eventDictonary.TryGetValue(eventName, out List<Action<object[]>> listeners))
        {
            listeners.Remove(listener);
        }
    }

    public static void TriggerEvent(string eventName, params object[] args)
    {
        if (eventDictonary.TryGetValue(eventName, out List<Action<object[]>> listeners))
        {
            foreach (var listener in listeners)
            {
                listener.Invoke(args);
            }
        }
    }

    public  static void RemoveAllEvents()
    {
        eventDictonary.Clear();
    }
}

