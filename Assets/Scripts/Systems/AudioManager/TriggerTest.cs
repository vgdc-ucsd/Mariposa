using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public StudioEventEmitter emitter;

    [Header("This just displays a header, no purpose")]
    public UnityEvent<float> unityFloatEvent;
    public SingleTriggerEvent singleTriggerEvent;

    //public List<FloatEvent> floatEvents;
    // when an event is invoked, it basically pokes at all stuff subscribed to it (calls all subscriber methods) Event.Invoke(possible value)
    // methods subscribe by unity inspector or hard coded by 

    void Start()
    {

    }

    public void Hello()
    {
        if (emitter != null && !emitter.IsPlaying())
        {
            emitter.Play();
        }
    }

    public void StopEvent()
    {
        if (emitter != null && emitter.IsPlaying())
        {
            emitter.Stop();
        }
    }
}

[System.Serializable]
public class FloatEvent : UnityEvent<float>
{
    private int hello;
    public int bye;

    public void floatEventTrigger()
    {
        return;
    }
}

[System.Serializable]
public class SingleTriggerEvent : UnityEvent<int>
{
    private int hello;
    public string goodbye;

    public void VoidTestMethod()
    {
        return;
    }

    public string StringTestMethodWithIntParameter(int value)
    {
        return value.ToString();
    }
}

public class EventTriggerer
{
    public void TriggerVoid()
    {
        return;
    }

    public int TriggerInt()
    {
        return 1;
    }
}

// i could create classes that have visible lists of all methods and commands that can be used
// AudioManager can have a visible list of enumerator to path / GUID but read only