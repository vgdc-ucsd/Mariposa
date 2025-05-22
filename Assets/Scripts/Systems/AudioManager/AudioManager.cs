using System;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private LevelManager levelManager => LevelManager.Instance;
    private AmbienceManager _ambienceManager = null;
    public AmbienceManager ambienceManager
    {
        get { return _ambienceManager; }
    }
    private MusicManager _musicManager = null;
    public MusicManager musicManager
    {
        get { return _musicManager; }
    }
    private VoicelineManager _voicelineManager = null;
    public VoicelineManager voicelineManager
    {
        get { return _voicelineManager; }
    }

    // if other managers not found, add them in with default settings
    void Start()
    {
        if (!TryGetComponent<AmbienceManager>(out _ambienceManager))
        {
            Debug.LogWarning("AmbienceManager not found! Attaching default AmbienceManager to AudioManager.");
            _ambienceManager = gameObject.AddComponent<AmbienceManager>();
        }

        if (!TryGetComponent<MusicManager>(out _musicManager))
        {
            Debug.LogWarning("MusicManager not found! Attaching default MusicManager to AudioManager.");
            _musicManager = gameObject.AddComponent<MusicManager>();
        }

        if (!TryGetComponent<VoicelineManager>(out _voicelineManager))
        {
            Debug.LogWarning("VoicelineManager not found! Attaching default VoicelineManager to AudioManager.");
            _voicelineManager = gameObject.AddComponent<VoicelineManager>();
        }
    }

    public Sublevel getCurrentSublevel()
    {
        Sublevel currentSublevel;
        try
        {
            currentSublevel = levelManager.CurrentLevel.Sublevels[levelManager.SublevelIndex];
            // Debug.Log("Sublevel index found: " + levelManager.SublevelIndex);
        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Unable to grab current sublevel from LevelManager! Attempting to use previous value.");
            return null;
        }
        return currentSublevel;
    }
}
