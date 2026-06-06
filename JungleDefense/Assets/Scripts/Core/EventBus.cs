using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> Subscribers = new();

    public static void Subscribe<T>(Action<T> callback)
    {
        if (callback == null)
        {
            return;
        }

        Type eventType = typeof(T);

        if (Subscribers.TryGetValue(eventType, out Delegate existing))
        {
            Subscribers[eventType] = Delegate.Combine(existing, callback);
        }
        else
        {
            Subscribers[eventType] = callback;
        }
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        if (callback == null)
        {
            return;
        }

        Type eventType = typeof(T);

        if (!Subscribers.TryGetValue(eventType, out Delegate existing))
        {
            return;
        }

        Delegate current = Delegate.Remove(existing, callback);

        if (current == null)
        {
            Subscribers.Remove(eventType);
        }
        else
        {
            Subscribers[eventType] = current;
        }
    }

    public static void Raise<T>(T gameEvent)
    {
        Type eventType = typeof(T);

        if (!Subscribers.TryGetValue(eventType, out Delegate existing))
        {
            return;
        }

        if (existing is Action<T> action)
        {
            action.Invoke(gameEvent);
        }
    }

    public static void Clear()
    {
        Subscribers.Clear();
    }
}
