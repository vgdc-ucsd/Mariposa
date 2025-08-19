using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using static AudioEvents;

public class AmbienceManager : Singleton<AmbienceManager>
{
    public enum Ambience
    {
        NONE,
        Tutorial_mariposa,
        Tutorial_unnamed,
        Downtown_mariposa,
        Downtown_unnamed,
        Hometown_mariposa,
        Hometown_unnamed,
    };

    [SerializeField] private EventReference AmbienceEvent;
    public EventInstance AmbienceEventInstance { get; private set; }
    public FMOD.Studio.STOP_MODE StopMode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT;

    public EventReference GetEventReference(Ambience ambience)
    {
        return ambience switch
        {
            Ambience.Tutorial_mariposa => AudioEvents.Ambience.s0Tutorial_mariposa,
            Ambience.Tutorial_unnamed => AudioEvents.Ambience.s0Tutorial_unnamed,
            Ambience.Downtown_mariposa => AudioEvents.Ambience.s1Downtown_mariposa,
            Ambience.Downtown_unnamed => AudioEvents.Ambience.s1Downtown_unnamed,
            Ambience.Hometown_mariposa => AudioEvents.Ambience.s4Hometown_mariposa,
            Ambience.Hometown_unnamed => AudioEvents.Ambience.s4Hometown_unnamed,
            _ => throw new ArgumentException($"{ambience} does not correspond to a valid EventReference")
        };
    }

    private bool CreateInstance()
    {
        if (AmbienceEvent.IsNull) return false;

        AmbienceEventInstance = RuntimeManager.CreateInstance(AmbienceEvent);
        return AmbienceEventInstance.isValid();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (IsPlaying()) return;

        //Stop();
        if (CreateInstance()) AmbienceEventInstance.start();
        else LogError();
    }

    [ContextMenu("Stop")]
    public void Stop()
    {
        if (!IsPlaying()) return;

        if (AmbienceEventInstance.isValid()) AmbienceEventInstance.stop(StopMode);
        else LogError();
    }

    public void ChangeAmbience(Ambience ambience)
    {
        if (ambience == Ambience.NONE)
        {
            Debug.Log("Tried changing ambience to Ambience.NONE, stopping ambience instead!");
            Stop();
            return;
        }

        ChangeAmbience(GetEventReference(ambience));
    }

    public void ChangeAmbience(EventReference ambienceEvent)
    {
        AmbienceEvent = ambienceEvent;

        Stop();
        Play();
    }

    private bool IsPlaying()
    {
        if (AmbienceEventInstance.isValid())
        {
            AmbienceEventInstance.getPlaybackState(out PLAYBACK_STATE state);
            return state != PLAYBACK_STATE.STOPPED && state != PLAYBACK_STATE.STOPPING;
        }
        return false;
    }

    // runs when a command could not execute
    private void LogError()
    {
        if (AmbienceEvent.IsNull)
        {
            Debug.LogWarning("No event currently selected! Stopping ambience instead!");
            Stop();
        }
        else if (!AmbienceEventInstance.isValid())
        {
            Debug.LogError("Invalid event instance!");
        }
        else
        {
            Debug.LogError("An unknown error has occurred!");
        }
    }

    private void OnDisable()
    {
        Stop();
    }
}
