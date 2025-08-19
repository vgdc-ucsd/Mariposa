using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    public EventInstance currentEventInstance { get; private set; }
    private EventInstance transitionEventInstance;
    private Bus musicBus;

    private EventReference currentMusicEvent;
    private Coroutine transitionCoroutine;

    private const float DEFAULT_TRANSITION_DURATION = 1.5f;

    public enum Music
    {
        NONE,
        Tutorial_mariposa,
        Tutorial_unnamed,
        Downtown_mariposa,
        Downtown_unnamed,
        Pier_mariposa,
        Pier_unnamed,
        BigRobot_unnamed,
        Hometown_mariposa,
        Hometown_unnamed,
        titlescreen_title,
    };

    private void Start()
    {
        musicBus = RuntimeManager.GetBus("bus:/Music");
        transitionCoroutine = null;
        currentMusicEvent = default;
    }

    public EventReference GetEventReference(Music music)
    {
        return music switch
        {
            Music.Tutorial_mariposa => AudioEvents.Music.s0Tutorial_mariposa,
            Music.Tutorial_unnamed => AudioEvents.Music.s0Tutorial_unnamed,
            Music.Downtown_mariposa => AudioEvents.Music.s1Downtown_mariposa,
            Music.Downtown_unnamed => AudioEvents.Music.s1Downtown_unnamed,
            Music.Pier_mariposa => AudioEvents.Music.s2Pier_mariposa,
            Music.Pier_unnamed => AudioEvents.Music.S2Pier_unnamed,
            Music.BigRobot_unnamed => AudioEvents.Music.s3BigRobot_unnamed,
            Music.Hometown_mariposa => AudioEvents.Music.s4Hometown_mariposa,
            Music.Hometown_unnamed => AudioEvents.Music.s4Hometown_unnamed,
            Music.titlescreen_title => AudioEvents.Music.titlescreen_title,
            _ => throw new ArgumentException($"{music} does not correspond to a valid EventReference")
        };
    }

    private bool IsPlaying()
    {
        if (currentEventInstance.isValid())
        {
            currentEventInstance.getPlaybackState(out PLAYBACK_STATE state);
            return state != PLAYBACK_STATE.STOPPED && state != PLAYBACK_STATE.STOPPING;
        }
        return false;
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (IsPlaying() || transitionCoroutine != null) return;

        if (currentMusicEvent.IsNull || currentMusicEvent.Equals(default))
        {
            Debug.LogError("Tried to play music with an empty music eventReference");
            return;
        }

        if (!currentEventInstance.isValid())
        {
            currentEventInstance = AudioEvents.CreateEventInstance(currentMusicEvent);
            if (!currentEventInstance.isValid()) return;
        }

        currentEventInstance.start();
    }

    [ContextMenu("Stop")]
    public void Stop(FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.IMMEDIATE)
    {
        if (IsPlaying())
        {
            currentEventInstance.stop(stopMode);
            currentEventInstance.release();
        }

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionEventInstance.stop(stopMode);
            transitionEventInstance.release();
            transitionEventInstance = default;
        }
    }


    public void ChangeMusic(Music musicEvent, float transitionDuration = DEFAULT_TRANSITION_DURATION)
    {
        if (!Enum.IsDefined(typeof(Music), musicEvent) || musicEvent == Music.NONE) Stop();
        else ChangeMusic(GetEventReference(musicEvent), transitionDuration);
    }

    public void ChangeMusic(EventReference musicEvent, float transitionDuration = DEFAULT_TRANSITION_DURATION)
    {
        if (transitionDuration < 0)
        {
            Debug.LogError("Music transition duration must be >= 0 seconds. Aborting...");
            return;
        }

        if (currentEventInstance.isValid() && currentMusicEvent.Equals(musicEvent)) return;

        if (!IsPlaying())
        {
            currentMusicEvent = musicEvent; 
            currentEventInstance = AudioEvents.CreateEventInstance(musicEvent);
            Play();
            return;
        }

        // Already playing music, so do a crossfade transition

        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(DoCrossfade(musicEvent, transitionDuration));
    }

    /*
    [ContextMenu("Transition Debug")]
    private void InspectorTransition()
    {
        ChangeMusic(transitionMusicEvent, 3.0f);
    }
    */

    public void SetVolume(float newVolume)
    {
        if (newVolume > 1.0 || newVolume < 0.0)
        {
            Debug.LogError("MusicManager volume must be set between 0.0 and 1.0");
            return;
        }

        musicBus.setVolume(newVolume);
    }

    private IEnumerator DoCrossfade(EventReference nextTrack, float duration)
    {
        transitionEventInstance = AudioEvents.CreateEventInstance(nextTrack);
        if (transitionEventInstance.Equals(default)) yield break;
        transitionEventInstance.setVolume(0.0f);
        transitionEventInstance.start();

        float elapsed = 0.0f;
        while (elapsed <= duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            float transitionPercent = Mathf.SmoothStep(0.0f, 1.0f, t);

            currentEventInstance.setVolume(1.0f - transitionPercent);
            transitionEventInstance.setVolume(transitionPercent);

            yield return null;
        }

        if (currentEventInstance.isValid())
        {
            currentEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentEventInstance.release();
        }

        currentEventInstance = transitionEventInstance;
        transitionEventInstance = default;
        currentMusicEvent = nextTrack;

        transitionCoroutine = null;
    }

    private void OnDisable()
    {
        Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
