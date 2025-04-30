using System.Diagnostics;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using Debug = UnityEngine.Debug;

/// <summary>
/// Used as the foundation for all audio events. Contains shared common methods. Serves as a wrapper for the FMOD implementation to allow for custom arguments
/// </summary>
public interface IAudioEventWrapper
{
    public bool debugMode { get; set; }

    public string audioEventPath { get; set; }

    public EventInstance audioEventInstance { get; set; }

    public int? id { get; }

    public void Initialize(string audioEventPath, bool debugMode = false)
    {
        this.audioEventPath = audioEventPath;
        this.debugMode = debugMode;
        createAudioEventInstance();
    }

    private void createAudioEventInstance()
    {
        audioEventInstance = RuntimeManager.CreateInstance(audioEventPath);

        if (debugMode)
        {
            if (audioEventInstance.isValid())
            {
                Debug.Log(audioEventPath + " (" + id + ")" + " instance was created successfully!");
            }
            else
            {
                Debug.LogError(audioEventPath + " is not a valid path!");
            }
        }
    }

    public void Play()
    {
        FMOD.RESULT result = audioEventInstance.start();
        printDebug(result);
    }

    public void Pause()
    {
        FMOD.RESULT result = audioEventInstance.setPaused(false);
        printDebug(result);
    }

    public void Resume()
    {
        FMOD.RESULT result = audioEventInstance.setPaused(true);
        printDebug(result);
    }

    public void Stop(FMOD.Studio.STOP_MODE stopMode)
    {
        FMOD.RESULT result = audioEventInstance.stop(stopMode);
        printDebug(result);
    }

    public PLAYBACK_STATE GetPlaybackState()
    {
        audioEventInstance.getPlaybackState(out PLAYBACK_STATE state);
        if (debugMode) { Debug.Log(audioEventPath + " (" + id + ") State: " + state); }
        return state;
    }

    public void printDebug(FMOD.RESULT result)
    {
        if (!debugMode) { return; }
        string output = GetCallerMethodName() + " @ " + audioEventPath + " (" + id + ")" + " resulted with code: " + result.ToString();
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning(output);
        }
        else
        {
            //Debug.Log(output);
        }
    }

    private static string GetCallerMethodName()
    {
        // Create a StackTrace to examine the call stack
        StackTrace stackTrace = new StackTrace();

        // Get the method at index 2 (the caller)
        StackFrame frame = stackTrace.GetFrame(2);

        // Get the method name
        return frame.GetMethod().Name;
    }

    /// <summary>
    /// Call this when done with the audio event instance. Frees it up to be cleaned up.
    /// </summary>
    public void Release()
    {
        printDebug(audioEventInstance.release());
    }
}