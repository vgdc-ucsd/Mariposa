using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>, IDataPersistence
{
    public MusicManager musicManager { get; private set; }
    public AmbienceManager ambienceManager { get; private set; }
    public VoicelineManager voicelineManager { get; private set; }

    public Bus masterBus { get; private set; }
    public Bus musicBus { get; private set; }
    public Bus sfxBus { get; private set; }
    public Bus ambienceBus { get; private set; }
    public Bus dialogueBus { get; private set; }

    [Range(0.0f, 1.0f)] public float masterVolume = 1.0f;
    [Range(0.0f, 1.0f)] public float musicVolume = 1.0f;
    [Range(0.0f, 1.0f)] public float sfxVolume = 1.0f;
    [Range(0.0f, 1.0f)] public float ambienceVolume = 1.0f;
    [Range(0.0f, 1.0f)] public float dialogueVolume = 1.0f;

    [Range(0.0f, 1.0f)] public float defaultMasterVolume = 0.5f;
    [Range(0.0f, 1.0f)] public float defaultMusicVolume = 0.5f;
    [Range(0.0f, 1.0f)] public float defaultSFXVolume = 0.5f;
    [Range(0.0f, 1.0f)] public float defaultAmbienceVolume = 0.5f;
    [Range(0.0f, 1.0f)] public float defaultDialogueVolume = 0.5f;

    private bool areBussesInitialized;

    void Start()
    {
        // if other managers not found, add them in with default settings
        if (!TryGetComponent<AmbienceManager>(out _))
        {
            Debug.LogWarning("AmbienceManager not found! Attaching default AmbienceManager to AudioManager.");
            ambienceManager = gameObject.AddComponent<AmbienceManager>();
        }

        if (!TryGetComponent<MusicManager>(out _))
        {
            Debug.LogWarning("MusicManager not found! Attaching default MusicManager to AudioManager.");
            musicManager = gameObject.AddComponent<MusicManager>();
        }

        if (!TryGetComponent<VoicelineManager>(out _))
        {
            Debug.LogWarning("VoicelineManager not found! Attaching default VoicelineManager to AudioManager.");
            voicelineManager = gameObject.AddComponent<VoicelineManager>();
        }

        StartCoroutine(LoadBusses());
    }

    private IEnumerator LoadBusses()
    {
        areBussesInitialized = false;

        while (!RuntimeManager.HaveAllBanksLoaded) yield return null;

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        dialogueBus = RuntimeManager.GetBus("bus:/Dialogue");

        if (!GameManager.Instance.hasGameDataBeenLoaded)
        {
            masterVolume = defaultMasterVolume;
            musicVolume = defaultMusicVolume;
            sfxVolume = defaultSFXVolume;
            ambienceVolume = defaultAmbienceVolume;
            dialogueVolume = defaultDialogueVolume;
        }

        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        sfxBus.setVolume(sfxVolume);
        ambienceBus.setVolume(ambienceVolume);
        dialogueBus.setVolume(dialogueVolume);

        areBussesInitialized = true;
    }

    public static EventInstance CreateEventInstance(EventReference eventReference)
    {
        if (eventReference.IsNull || eventReference.Equals(default(EventReference)))
        {
            Debug.LogError("Tried to create an event instance from an invalid event");
            return default;
        }

        EventInstance newEventInstance = RuntimeManager.CreateInstance(eventReference);
        if (!newEventInstance.isValid())
        {
            Debug.LogError($"Failed to create a valid event instance for {eventReference.Guid}");
            return default;
        }

        return newEventInstance;
    }

    public static void StopEventInstance(EventInstance eventInstance, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.IMMEDIATE)
    {
        if (!eventInstance.isValid()) return;

        eventInstance.stop(stopMode);
        eventInstance.release();
    }

    public static bool IsPlaying(EventInstance eventInstance) {
        PLAYBACK_STATE state;   
        eventInstance.getPlaybackState(out state);
        return state != PLAYBACK_STATE.STOPPED;
    }

    #region SetVolumes
    private void SetVolume(Bus bus, float newVolume)
    {
        if (!areBussesInitialized) return;
        bus.setVolume(newVolume);
    }

    public void SetMasterVolume(float newVolume)
    {
        if (newVolume > 1.0 || newVolume < 0.0)
        {
            Debug.LogError("Volume must be set between 0.0 and 1.0");
            return;
        }

        masterVolume = newVolume;
        SetVolume(masterBus, newVolume);
    }

    public void SetMusicVolume(float newVolume)
    {
        if (!areBussesInitialized) return;
        if (newVolume > 1.0 || newVolume < 0.0)
        {
            Debug.LogError("Volume must be set between 0.0 and 1.0");
            return;
        }

        musicVolume = newVolume;
        SetVolume(musicBus, newVolume);
    }

    public void SetSFXVolume(float newVolume)
    {
        if (newVolume > 1.0 || newVolume < 0.0)
        {
            Debug.LogError("Volume must be set between 0.0 and 1.0");
            return;
        }

        sfxVolume = newVolume;
        SetVolume(sfxBus, newVolume);
    }

    public void SetAmbienceVolume(float newVolume)
    {
        if (newVolume > 1.0 || newVolume < 0.0)
        {
            Debug.LogError("Volume must be set between 0.0 and 1.0");
            return;
        }

        ambienceVolume = newVolume;
        SetVolume(ambienceBus, newVolume);
    }

    public void SetDialogueVolume(float newVolume)
    {
        if (newVolume > 1.0 || newVolume < 0.0)
        {
            Debug.LogError("Volume must be set between 0.0 and 1.0");
            return;
        }

        dialogueVolume = newVolume;
        SetVolume(dialogueBus, newVolume);
    }
    #endregion

    [System.Flags]
    public enum PauseTypes
    {
        Master = 1 << 0,
        Music = 1 << 1,
        SFX = 1 << 2,
        Ambience = 1 << 3,
        Dialogue = 1 << 4,
    };

    public void ToggleGlobalPause(bool shouldPause, PauseTypes pauseBusFlags)
    {
        if ((pauseBusFlags & PauseTypes.Master) != 0) masterBus.setPaused(shouldPause);
        if ((pauseBusFlags & PauseTypes.Music) != 0) musicBus.setPaused(shouldPause);
        if ((pauseBusFlags & PauseTypes.SFX) != 0) sfxBus.setPaused(shouldPause);
        if ((pauseBusFlags & PauseTypes.Ambience) != 0) ambienceBus.setPaused(shouldPause);
        if ((pauseBusFlags & PauseTypes.Dialogue) != 0) dialogueBus.setPaused(shouldPause);
    }

    public void SaveData(ref GameData data)
    {
        data.masterValue = masterVolume;
        data.musicValue = musicVolume;
        data.sfxValue = sfxVolume;
        data.ambienceValue = ambienceVolume;
        data.dialogueValue = dialogueVolume;
    }

    public void LoadData(GameData data)
    {
        masterVolume = data.masterValue;
        musicVolume = data.musicValue;
        sfxVolume = data.sfxValue;
        ambienceVolume = data.ambienceValue;
        dialogueVolume = data.dialogueValue;

        if (areBussesInitialized)
        {
            masterBus.setVolume(masterVolume);
            musicBus.setVolume(musicVolume);
            sfxBus.setVolume(sfxVolume);
            ambienceBus.setVolume(ambienceVolume);
            dialogueBus.setVolume(dialogueVolume);
        }
    }
}
