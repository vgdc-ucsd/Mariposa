using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class VoicelineManager : Singleton<VoicelineManager>
{
    [SerializeField] private VoicelineEventSO currentVoiceline;
    private EventInstance currentVoicelineInstance;
    public bool isPlaying => checkPlaying();

    public void setVoiceline(VoicelineEventSO voiceline)
    {
        // Validate input
        if (voiceline == null)
        {
            Debug.LogError("Attempted to set null voiceline. No changes will be applied.");
            return;
        }

        // Release previous instance if it exists
        if (currentVoicelineInstance.isValid())
        {
            currentVoicelineInstance.release();
        }

        currentVoiceline = voiceline;
        currentVoicelineInstance = RuntimeManager.CreateInstance(currentVoiceline);
    }

    [ContextMenu("Inspector Update")]
    public void UpdateFromInspector()
    {
        setVoiceline(currentVoiceline);
    }

    [ContextMenu("Play")]
    public void play()
    {
        if (!checkValid()) { return; }
        currentVoicelineInstance.start();
    }

    [ContextMenu("Stop")]
    public void stop()
    {
        if (!checkValid()) { return; }
        currentVoicelineInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private bool checkValid()
    {
        if (currentVoicelineInstance.isValid())
        {
            return true;
        }

        Debug.LogError("Current voiceline audio is not valid!");
        return false;
    }

    private bool checkPlaying()
    {
        if (!checkValid()) { return false; }

        currentVoicelineInstance.getPlaybackState(out PLAYBACK_STATE state);

        return state != PLAYBACK_STATE.STOPPED;
    }

    private void OnDestroy()
    {
        if (currentVoicelineInstance.isValid())
        {
            stop();
            currentVoicelineInstance.release();
        }
    }
}