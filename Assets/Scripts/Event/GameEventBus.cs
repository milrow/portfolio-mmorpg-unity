using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEventBus
{   
    private static readonly Dictionary<Type, Delegate> handlers = new();

    public static void Subscribe<T>(Action<T> handler) where T : IGameEvent
    {
        var type = typeof(T);

        if (handlers.TryGetValue(type, out var existing)) 
        {
            handlers[type] = Delegate.Combine(existing, handler);
        }
        else
        {
            handlers[type] = handler;
        }
    }

    public static void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
    {
        var type = typeof(T);

        if(!handlers.TryGetValue(type, out var existing)) return;

        var current = Delegate.Remove(existing, handler);

        if (current == null) 
        {
            handlers.Remove(type);
        }
        else
        {
            handlers[type] = current;
        }
    }

    public static void Publish<T>(T gameEvent) where T : IGameEvent
    { 
        if(handlers.TryGetValue(typeof(T), out var handler))
        {
            ((Action<T>)handler)?.Invoke(gameEvent);
        }
    }
}
