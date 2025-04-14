using System;
using System.Collections.Generic;

namespace Yjsnpi.Events;

public static class EventManager
{
    private static class EventType<T> where T : struct
    {
        public static readonly List<Action<T>> Listeners = new List<Action<T>>(4);
    }
    
    public static void Subscribe<T>(Action<T> handler) where T : struct
    {
        EventType<T>.Listeners.Add(handler);
    }
    
    public static void Send<T>(in T eventData) where T : struct
    {
        foreach (var handler in EventType<T>.Listeners)
        {
            handler?.Invoke(eventData);
        }
    }
    
    public static void Clear<T>() where T : struct
    {
        EventType<T>.Listeners.Clear();
    }
}
