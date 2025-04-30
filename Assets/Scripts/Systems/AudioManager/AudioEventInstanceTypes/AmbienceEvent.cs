using System.Collections;
using FMOD.Studio;
using UnityEngine;
using static AudioManager;

// this class is meant to handle
public class AmbienceEvent : Singleton<AmbienceEvent>, IAudioEventWrapper
{
    // NOTE: a lot of default implementation has already been done in the interface AudioEventWrapper
    public IAudioEventWrapper DefaultControls => this;

    public bool debugMode { get; set; }
    public string audioEventPath { get; set; }
    public EventInstance audioEventInstance { get; set; }
    private int? _id;
    public int? id
    {
        get
        {
            if (_id == null)
            {
                _id = GetHashCode();
            }
            return _id;
        }
    }


    // the code here only serves the {CLASS_NAME}-exclusive methods

    // TODO: how do i handle methods that are subclasses of this
    public void EventTest()
    {
        Debug.Log("This is a certifiable " + GetType().Name + " moment!");
    }

    public void ChangeAmbience(Audio_Ambience newAmbience, STOP_MODE oldAmbienceStopMode = STOP_MODE.ALLOWFADEOUT)
    {
        EventInstance oldAmbience = ((IAudioEventWrapper)this).audioEventInstance;

        DefaultControls.Stop(oldAmbienceStopMode);
        StartCoroutine(FinishStoppingBeforeRelease(oldAmbience));
        AudioManager.Instance.createAudioEvent(newAmbience, DefaultControls.debugMode);
    }

    private static IEnumerator FinishStoppingBeforeRelease(EventInstance instance)
    {
        PLAYBACK_STATE state;
        do
        {
            yield return null;
            instance.getPlaybackState(out state);
        } while (state != PLAYBACK_STATE.STOPPED);

        instance.release();
    }
}