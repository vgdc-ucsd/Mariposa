using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private Bus currentBus, transitionBus;
    private Bus music1Bus = RuntimeManager.GetBus("bus:/Music/Music1");
    private Bus music2Bus = RuntimeManager.GetBus("bus:/Music/Music2");
    private EventInstance currentEventInstance;
    private EventInstance? transitionEventInstance;
    private float transitionPercent = 0.0f;

    public EventReference currentMusic;
    public EventReference? transitionMusic = null;

    private void Start()
    {
        currentBus = music1Bus;
        transitionBus = music2Bus;
        transitionPercent = 0.0f;

        // start playing music if one already exists and playOnStart is on
    }

    private void CreateInstances()
    {

    }

    public void TransitionTo(string path)
    {
        transitionMusic = RuntimeManager.PathToEventReference(path);
        transitionEventInstance = RuntimeManager.CreateInstance((EventReference)transitionMusic);
        // start lowering current music and raising other music
        // when done, stop current music, release, transition -> current, clear transition
    }

    private IEnumerator SongTransition(float duration)
    {
        yield return StartCoroutine(VolumeTransition(duration));
        transitionPercent = 0.0f;
        swap();
    }

    private IEnumerator VolumeTransition(float duration)
    {
        float elapsed = 0.0f;
        while (elapsed <= 1.0f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transitionPercent = Mathf.Lerp(0.0f, 1.0f, t);
            setTransitionVolume();
            yield return null;
        }
    }

    // change all internal variables to reflect aftereffects of music transition
    private void swap()
    {
        // swap buses
        if (music1Bus.Equals(currentBus))
        {
            currentBus = music2Bus;
            transitionBus = music1Bus;
        }
        else
        {
            currentBus = music1Bus;
            transitionBus = music2Bus;
        }

        // delete current event, move transition -> current, delete old transition
        currentEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        currentEventInstance.release();
        currentEventInstance = (EventInstance)transitionEventInstance;
        transitionEventInstance = null;

        // same with EventReferences
        currentMusic = (EventReference)transitionMusic;
        transitionMusic = null;
    }

    private void setTransitionVolume()
    {
        currentBus.setVolume(1.0f - transitionPercent);
        transitionBus.setVolume(transitionPercent);
    }
}
