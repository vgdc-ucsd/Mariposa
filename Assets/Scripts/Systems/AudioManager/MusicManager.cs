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
        getCurrentSublevelEvent();
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

    private void getCurrentSublevelEvent()
    {
        Sublevel currentSublevel = AudioManager.Instance.getCurrentSublevel();
        MusicEvent = currentSublevel != null ? currentSublevel.SublevelMusic : Music.NONE;
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (isPlaying()) { return; }

        //Stop();
        createInstance();

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

    public void ChangeEvent()
    {
        Stop();
        createInstance();
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
