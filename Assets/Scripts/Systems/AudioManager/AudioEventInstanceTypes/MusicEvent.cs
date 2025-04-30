using System.Collections;
using FMOD.Studio;
using Unity.Collections;
using UnityEngine;
using static AudioManager;
public class MusicEvent : Singleton<MusicEvent>, IAudioEventWrapper
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

    public void ChangeMusic(Audio_Music newMusic, STOP_MODE oldMusicStopMode = STOP_MODE.ALLOWFADEOUT)
    {
        EventInstance oldMusic = ((IAudioEventWrapper)this).audioEventInstance;

        DefaultControls.Stop(oldMusicStopMode);
        StartCoroutine(FinishStoppingBeforeRelease(oldMusic));
        AudioManager.Instance.createAudioEvent(newMusic, DefaultControls.debugMode);
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