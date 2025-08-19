using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

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

    private EventReference ambienceEvent;
    public EventInstance ambienceEventInstance { get; private set; }
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

    [ContextMenu("Play")]
    public void Play()
    {
        if (IsPlaying()) return;

        if (ambienceEvent.IsNull || ambienceEvent.Equals(default))
        {
            Debug.LogError("Tried to play ambience with an empty ambience eventReference");
            return;
        }

        if (!ambienceEventInstance.isValid())
        {
            ambienceEventInstance = AudioEvents.CreateEventInstance(ambienceEvent);
            if (!ambienceEventInstance.isValid()) return;
        }

        ambienceEventInstance.start();
    }

    [ContextMenu("Stop")]
    public void Stop()
    {
        if (!IsPlaying()) return;

        if (ambienceEventInstance.isValid())
        {
            ambienceEventInstance.stop(StopMode);
            ambienceEventInstance.release();
        }
        else LogError();
    }

    public void ChangeAmbience(Ambience ambience)
    {
        if (!Enum.IsDefined(typeof(Ambience), ambience) || ambience == Ambience.NONE) Stop();
        else ChangeAmbience(GetEventReference(ambience));
    }

    public void ChangeAmbience(EventReference ambienceEvent)
    {
        this.ambienceEvent = ambienceEvent;

        Stop();
        Play();
    }

    private bool IsPlaying()
    {
        if (ambienceEventInstance.isValid())
        {
            ambienceEventInstance.getPlaybackState(out PLAYBACK_STATE state);
            return state != PLAYBACK_STATE.STOPPED && state != PLAYBACK_STATE.STOPPING;
        }
        return false;
    }

    // runs when a command could not execute
    private void LogError()
    {
        if (ambienceEvent.IsNull)
        {
            Debug.LogWarning("No event currently selected! Stopping ambience instead!");
            Stop();
        }
        else if (!ambienceEventInstance.isValid())
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
