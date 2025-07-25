using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    private T state;
    private Dictionary<T, Action> onEnterActions;
    private Dictionary<T, Action> onExitActions;
    private HashSet<Tuple<T, T>> transitions;
    private Dictionary<Tuple<T, T>, Action> transitionActions;

    public StateMachine(T initState)
    {
        state = initState;
        onEnterActions = new Dictionary<T, Action>();
        onExitActions = new Dictionary<T, Action>();
        transitions = new HashSet<Tuple<T, T>>();
        transitionActions = new Dictionary<Tuple<T, T>, Action>();
    }

    public T GetState() { return state; }

    public void AddEnterAction(T state, Action action)
    {
        onEnterActions.Add(state, action);
    }

    public void RemoveEnterAction(T state)
    {
        onEnterActions.Remove(state);
    }

    public void AddExitAction(T state, Action action)
    {
        onExitActions.Add(state, action);
    }

    public void RemoveExitAction(T state)
    {
        onExitActions.Remove(state);
    }

    public void AddTransition(T state, T newState)
    {
        transitions.Add(Tuple.Create(state, newState));
    }

    public void AddTransitionAction(T state, T newState, Action action)
    {
        Tuple<T, T> transition = Tuple.Create(state, newState);
        transitionActions.Add(transition, action);
    }

    public void Transition(T newState)
    {
        Tuple<T, T> transition = Tuple.Create(state, newState);
        if (!transitions.Contains(transition))
        {
            Debug.LogWarning($"Tried to transition state from {state} to {newState}!");
            return;
        }

        if (onExitActions.ContainsKey(state))
        {
            onExitActions[state].Invoke();
        }

        if (transitionActions.ContainsKey(transition))
        {
            transitionActions[transition].Invoke();
        }

        if (onEnterActions.ContainsKey(newState))
        {
            onEnterActions[newState].Invoke();
        }

        state = newState;
    }
}
