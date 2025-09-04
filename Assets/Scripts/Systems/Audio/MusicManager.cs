using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using static AudioEvents;

public class MusicManager : Singleton<MusicManager>
{
    private string currentMusicPath, transitionMusicPath = "";
    [SerializeField] private Music currentMusicEvent = Music.NONE;
    [SerializeField] private Music transitionMusicEvent = Music.NONE;

    private Bus music1Bus, music2Bus;

    public EventInstance currentEventInstance { get; private set; }
    private EventInstance transitionEventInstance;

    private float transitionPercent = 0.0f;
    private float elapsed = 0.0f;
    private bool isTransitionValid = false;
    private bool isCurrentlyTransitioning = false;
    public bool PlayOnStart = false;

    private const float DEFAULT_TRANSITION_DURATION = 1.5f;

    private enum busOptions
    {
        music1,
        music2
    }
    private busOptions busChoice;

    private void Start()
    {
        busChoice = busOptions.music1;
        music1Bus = RuntimeManager.GetBus("bus:/Music/Music1");
        music2Bus = RuntimeManager.GetBus("bus:/Music/Music2");
        transitionPercent = 0.0f;
        setTransitionVolume();
        updatePath();

        if (PlayOnStart)
        {
            Play();
        }
    }

    private Bus getCurrentBus()
    {
        if (busChoice == busOptions.music1)
        {
            return music1Bus;
        }
        else
        {
            return music2Bus;
        }
    }

    private Bus getTransitionBus()
    {
        if (busChoice != busOptions.music1)
        {
            return music1Bus;
        }
        else
        {
            return music2Bus;
        }
    }

    private bool isPlaying()
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
        if (isPlaying()) { return; }

        if (currentMusicEvent.IsValid())
        {
            currentMusicPath = currentMusicEvent.GetPath();
            updatePath();
            currentEventInstance = RuntimeManager.CreateInstance(currentMusicPath);
        }
        else
        {
            Debug.LogWarning($"Invalid music for '{currentMusicEvent}': Skipping Play()");
            return;
        }

        currentEventInstance.start();

        if (isTransitionValid)
        {
            transitionEventInstance.start();
        }
    }

    [ContextMenu("Stop")]
    public void Stop(FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.IMMEDIATE)
    {
        if (!isPlaying()) { return; }

        currentEventInstance.stop(stopMode);

        if (isTransitionValid)
        {
            transitionEventInstance.stop(stopMode);
        }
    }

    public void ChangeMusic(Music transitionEvent, float duration = DEFAULT_TRANSITION_DURATION)
    {
        if (duration < 0)
        {
            Debug.LogError("Music transition duration must be >= 0 seconds. Aborting...");
            return;
        }
        if (!transitionEvent.IsValid())
        {
            Debug.Log("No music exists! Stopping music instead!");
            Stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            return;
        }
        if (currentMusicEvent == Music.NONE)
        {
            currentMusicEvent = transitionEvent;
            updatePath();
            currentEventInstance = RuntimeManager.CreateInstance(currentMusicPath);
            if (!currentEventInstance.isValid())
            {
                Debug.LogError("Current event is not valid! Most likely caused by typo in AudioEvents.cs");
                return;
            }
            Play();
        }
        if (isCurrentlyTransitioning)
        {
            transitionEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            StopAllCoroutines();
            transitionMusicEvent = transitionEvent;
            transitionMusicPath = transitionMusicEvent.GetPath();
            updatePath();
            transitionEventInstance = RuntimeManager.CreateInstance(transitionMusicPath);
            if (!transitionEventInstance.isValid())
            {
                Debug.LogError("Transition Event is not valid! Most likely caused by typo in AudioEvents.cs. Aborting transition...");
                transitionPercent = 0.0f;
                setTransitionVolume();
            }
            transitionEventInstance.start();
            isTransitionValid = true;
            StartCoroutine(VolumeTransition(duration));
            return;
        }

        transitionMusicEvent = transitionEvent;
        transitionMusicPath = transitionMusicEvent.GetPath();
        updatePath();
        transitionEventInstance = RuntimeManager.CreateInstance(transitionMusicPath);
        if (!transitionEventInstance.isValid())
        {
            Debug.LogError("Transition Event is not valid! Most likely caused by typo in AudioEvents.cs");
            return;
        }
        isTransitionValid = true;
        StartCoroutine(SongTransition(duration));
    }

    [ContextMenu("Transition Debug")]
    private void InspectorTransition()
    {
        ChangeMusic(transitionMusicEvent, 3.0f);
    }

    public void SetVolume(float volume)
    {
        if (volume > 1.0 || volume < 0.0)
        {
            Debug.LogError("MusicManager volume must be set between 0.0 and 1.0");
            return;
        }

        getCurrentBus().setVolume(volume);
    }

    private IEnumerator SongTransition(float duration)
    {
        isCurrentlyTransitioning = true;
        transitionEventInstance.start();
        elapsed = 0.0f;
        yield return StartCoroutine(VolumeTransition(duration));
    }

    private IEnumerator VolumeTransition(float duration)
    {
        while (elapsed <= duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transitionPercent = Mathf.Lerp(0.0f, 1.0f, t);
            setTransitionVolume();
            yield return null;
        }
        transitionPercent = 0.0f;
        swap();
        isCurrentlyTransitioning = false;
    }

    // change all internal variables to reflect aftereffects of music transition
    private void swap()
    {
        // swap buses
        if (busChoice == busOptions.music1)
        {
            busChoice = busOptions.music2;
        }
        else
        {
            busChoice = busOptions.music1;
        }

        if (isTransitionValid)
        {
            currentEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentEventInstance.release();
            currentEventInstance = transitionEventInstance;

            // same with EventReferences and paths
            currentMusicEvent = transitionMusicEvent;
            transitionMusicEvent = Music.NONE;
        }
        updatePath();
        isTransitionValid = false;
    }

    private void setTransitionVolume()
    {
        getCurrentBus().setVolume(1.0f - transitionPercent);
        getTransitionBus().setVolume(transitionPercent);
    }

    private void updatePath()
    {
        if (busChoice == busOptions.music1)
        {
            if (currentMusicEvent.IsValid()) { currentMusicPath = currentMusicEvent.GetPath().Replace("music2", "music1"); }
            if (transitionMusicEvent.IsValid()) { transitionMusicPath = transitionMusicEvent.GetPath().Replace("music1", "music2"); }
        }
        else
        {
            if (currentMusicEvent.IsValid()) { currentMusicPath = currentMusicEvent.GetPath().Replace("music1", "music2"); }
            if (transitionMusicEvent.IsValid()) { transitionMusicPath = transitionMusicEvent.GetPath().Replace("music2", "music1"); }
        }
    }

    private void OnDisable()
    {
        Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
