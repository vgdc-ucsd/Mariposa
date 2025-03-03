using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class GameEvent
{
    private event Action OnEventTriggered;

    public virtual void Subscribe(Action listener) => OnEventTriggered += listener;
    public virtual void Unsubscribe(Action listener) => OnEventTriggered -= listener;
    public virtual void Trigger() => OnEventTriggered?.Invoke();
}

public class GameEvents
{
    private static readonly GameEvents _instance = new GameEvents();
    public static GameEvents Instance => _instance;

    private readonly Dictionary<Type, GameEvent> eventDictionary = new Dictionary<Type, GameEvent>();

    private void RegisterEvent<T>(T gameEvent) where T : GameEvent
    {
        eventDictionary[typeof(T)] = gameEvent;
    }

    public void Subscribe<T>(Action action) where T : GameEvent
    {
        if (eventDictionary.TryGetValue(typeof(T), out GameEvent gameEvent))
        {
            gameEvent.Subscribe(action);
        }
    }

    public void Unsubscribe<T>(Action action) where T : GameEvent
    {
        if (eventDictionary.TryGetValue(typeof(T), out GameEvent gameEvent))
        {
            gameEvent.Unsubscribe(action);
        }
    }

    public void Trigger<T>() where T : GameEvent
    {
        if (eventDictionary.TryGetValue(typeof(T), out GameEvent gameEvent))
        {
            gameEvent.Trigger();
        }
    }

    // add all events here
    private GameEvents() 
    {
        RegisterEvent(new Event1());
        RegisterEvent(new Event2());
        RegisterEvent(new UpdateTriggers());
    } 
}

public class Event1 : GameEvent { }
public class Event2 : GameEvent { }
public class UpdateTriggers : GameEvent 
{
    public override void Subscribe(Action listener)
    {
        base.Subscribe(listener);
    }

    public override void Trigger()
    {
        base.Trigger();
    }
}