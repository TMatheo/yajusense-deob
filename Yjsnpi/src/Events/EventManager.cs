using System;
using System.Collections.Generic;
using Yjsnpi.Core;

namespace Yjsnpi.Events;

public static class EventManager
{
    private static readonly Dictionary<Type, Delegate> _eventTable = new();

    public static void AddEventListener<T>(Action<T> handler)
    {
        if (handler == null)
        {
            YjPlugin.Log.LogError(nameof(handler) + " is null");
            return;
        }
        
        if (_eventTable.TryGetValue(typeof(T), out var existing))
        {
            _eventTable[typeof(T)] = Delegate.Combine(existing, handler);
        }
        else
        {
            _eventTable[typeof(T)] = handler;
        }
    }
    
    public static void RemoveEventListener<T>(Action<T> handler)
    {
        if (_eventTable.TryGetValue(typeof(T), out var existing))
        {
            _eventTable[typeof(T)] = Delegate.Remove(existing, handler);
        }
    }
    
    public static void Publish<T>(T eventData)
    {
        if (_eventTable.TryGetValue(typeof(T), out var action))
        {
            (action as Action<T>)?.Invoke(eventData);
        }
    }
}