using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using static AudioEvents;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] private Music MusicEvent = Music.NONE;
    public EventInstance MusicEventInstance { get; private set; }
    public FMOD.Studio.STOP_MODE StopMode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT;
    public bool PlayOnStart = false;
    private bool isValid = false;

    private void Start()
    {
        if (PlayOnStart) { Play(); }
    }

    private void createInstance()
    {
        if (MusicEvent != Music.NONE)
        {
            MusicEventInstance = RuntimeManager.CreateInstance(MusicEvent.GetPath());
            isValid = MusicEventInstance.isValid();
        }
        else
        {
            isValid = false;
        }
    }

    public Music GetMusicToCurrentSublevel()
    {
        Sublevel currentSublevel = AudioManager.Instance.getCurrentSublevel();
        if (currentSublevel == null)
        {
            return Music.NONE;
        }
        return currentSublevel.SublevelMusic;
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (isPlaying()) { return; }

        createInstance();

        // checks if instance is valid again
        if (isValid)
        {
            MusicEventInstance.start();
        }
        else
        {
            logHandler();
        }
    }

    [ContextMenu("Stop")]
    public void Stop()
    {
        if (!isPlaying()) { return; }

        if (isValid)
        {
            MusicEventInstance.stop(StopMode);
        }
        else
        {
            logHandler();
        }
    }

    public void ChangeMusic(Music musicEvent)
    {
        if (musicEvent == Music.NONE)
        {
            Debug.LogError("Tried changing music to Music.NONE, no changes will be applied!");
            return;
        }

        MusicEvent = musicEvent;

        Stop();
        Play();
    }

    private bool isPlaying()
    {
        if (isValid)
        {
            MusicEventInstance.getPlaybackState(out PLAYBACK_STATE state);
            return state != PLAYBACK_STATE.STOPPED && state != PLAYBACK_STATE.STOPPING;
        }
        return false;
    }

    // runs when a command could not execute
    private void logHandler()
    {
        if (MusicEvent == Music.NONE)
        {
            Debug.LogWarning("No event currently selected!");
        }
        else if (!isValid)
        {
            Debug.LogError("Invalid event!");
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
