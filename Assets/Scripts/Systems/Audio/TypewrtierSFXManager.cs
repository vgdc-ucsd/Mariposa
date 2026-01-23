using UnityEngine;
using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
public class TypewriterSFXManager : Singleton<TypewriterSFXManager>
{
    public EventInstance currentEventInstance {get;set;}

    [SerializeField] private EventReference currentTypeSFX;

    private Coroutine transitionCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public enum Speaker
    {
        Mariposa,
        Unnamed,
        General
    }
    void Start()
    {
        transitionCoroutine = null;
    }

    private bool IsPlaying()
    {
       if (currentEventInstance.isValid())
        {
            currentEventInstance.getPlaybackState(out PLAYBACK_STATE state);
            return state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.SUSTAINING || state == PLAYBACK_STATE.STOPPING;
        }
        return false;
    }

    public void Play()
    {
        if (!currentEventInstance.isValid())
        {
            currentEventInstance = AudioManager.CreateEventInstance(currentTypeSFX);
            if (!currentEventInstance.isValid()) return;
        }

        currentEventInstance.start();
    }

    public void Stop(FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.IMMEDIATE)
    {
        if(IsPlaying())
        {
            AudioManager.StopEventInstance(currentEventInstance);
            currentEventInstance = default;
            currentTypeSFX = default;
        }

        if(transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
    }

    public void ChangeTypeSFX(string name)
    {
        float speakerID;
        
        string unnamed = DataPersistenceManager.Instance.gameData.UnnamedName;
        
        if(name == "Mariposa")
        {
            speakerID = 0;
        }else if (name == unnamed)
        {
            speakerID = 1;
        }
        else
        {
            speakerID = 3;
        }

        //Debug.Log("Typewriter sfx changed to " + name +", Speaker ID: " + speakerID);
        currentEventInstance.setParameterByName("Speaker", speakerID);
    }

    private void OnDisable()
    {
        Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

}
