using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using static AudioEvents;

public class AmbienceManager : Singleton<AmbienceManager>
{
    [SerializeField] private Ambience AmbienceEvent = Ambience.NONE;
    public EventInstance AmbienceEventInstance { get; private set; }
    public FMOD.Studio.STOP_MODE StopMode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT;
    public bool PlayOnStart = false;
    private bool isValid = false;

    private void Start()
    {
        if (PlayOnStart) { Play(); }
    }

    private void createInstance()
    {
        if (AmbienceEvent != Ambience.NONE)
        {
            AmbienceEventInstance = RuntimeManager.CreateInstance(AmbienceEvent.GetPath());
            isValid = AmbienceEventInstance.isValid();
        }
        else
        {
            isValid = false;
        }
    }

    public Ambience GetAmbienceToCurrentSublevel()
    {
        Sublevel currentSublevel = AudioManager.Instance.getCurrentSublevel();
        if (currentSublevel == null)
        {
            return Ambience.NONE;
        }
        return currentSublevel.SublevelAmbience;
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (isPlaying()) { return; }

        //Stop();
        createInstance();

        if (isValid)
        {
            AmbienceEventInstance.start();
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
            AmbienceEventInstance.stop(StopMode);
        }
        else
        {
            logHandler();
        }
    }

    public void ChangeAmbience(Ambience ambienceEvent)
    {
        if (ambienceEvent == Ambience.NONE)
        {
            Debug.LogError("Tried changing ambience to Ambience.NONE, no changes will be applied!");
            return;
        }

        AmbienceEvent = ambienceEvent;

        Stop();
        Play();
    }

    private bool isPlaying()
    {
        if (isValid)
        {
            AmbienceEventInstance.getPlaybackState(out PLAYBACK_STATE state);
            return state != PLAYBACK_STATE.STOPPED && state != PLAYBACK_STATE.STOPPING;
        }
        return false;
    }

    // runs when a command could not execute
    private void logHandler()
    {
        if (AmbienceEvent == Ambience.NONE)
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
