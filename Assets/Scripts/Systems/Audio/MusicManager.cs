using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] private string currentMusic, transitionMusic = "";

    private Bus music1Bus, music2Bus;
    private EventInstance currentEventInstance, transitionEventInstance;

    private float transitionPercent = 0.0f;
    private bool transitionValid = false;
    [SerializeField] private bool playOnStart = false;

    [Header("Only used in Inspector Context Menu")]
    [SerializeField] private float transitionDuration = 3.0f;

    private enum busOptions
    {
        music1,
        music2
    }
    private busOptions busChoice;

    private void Start()
    {
        busChoice = busOptions.music1;
        music1Bus = RuntimeManager.GetBus("bus:/Music/Music1");
        music2Bus = RuntimeManager.GetBus("bus:/Music/Music2");
        transitionPercent = 0.0f;
        setTransitionVolume();
        updatePath();
        currentEventInstance = RuntimeManager.CreateInstance(currentMusic);
        if (playOnStart)
        {
            Play();
        }
    }

    private Bus getCurrentBus()
    {
        if (busChoice == busOptions.music1)
        {
            return music1Bus;
        }
        else
        {
            return music2Bus;
        }
    }

    private Bus getTransitionBus()
    {
        if (busChoice != busOptions.music1)
        {
            return music1Bus;
        }
        else
        {
            return music2Bus;
        }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        currentEventInstance.start();
        if (transitionValid)
        {
            transitionEventInstance.start();
        }
    }

    [ContextMenu("Stop")]
    public void Stop(FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.IMMEDIATE)
    {
        currentEventInstance.stop(stopMode);
        if (transitionValid)
        {
            transitionEventInstance.stop(stopMode);
        }
    }

    public void TransitionTo(string path, float duration)
    {
        transitionMusic = path;
        updatePath();
        transitionEventInstance = RuntimeManager.CreateInstance(transitionMusic);
        if (!transitionEventInstance.isValid())
        {
            Debug.LogError("Transition Event is not valid!");
            return;
        }
        transitionValid = true;
        StartCoroutine(SongTransition(duration));
    }

    [ContextMenu("Transition")]
    private void InspectorTransition()
    {
        TransitionTo(transitionMusic, transitionDuration);
    }

    private IEnumerator SongTransition(float duration)
    {
        transitionEventInstance.start();
        yield return StartCoroutine(VolumeTransition(duration));
        transitionPercent = 0.0f;
        swap();
    }

    private IEnumerator VolumeTransition(float duration)
    {
        float elapsed = 0.0f;
        while (elapsed <= duration)
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
        if (busChoice == busOptions.music1)
        {
            busChoice = busOptions.music2;
        }
        else
        {
            busChoice = busOptions.music1;
        }

        if (transitionValid)
        {
            currentEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentEventInstance.release();
            currentEventInstance = transitionEventInstance;

            // same with EventReferences
            currentMusic = transitionMusic;
            transitionMusic = "";

            transitionValid = false;
        }
        updatePath();
    }

    private void setTransitionVolume()
    {
        getCurrentBus().setVolume(1.0f - transitionPercent);
        getTransitionBus().setVolume(transitionPercent);
    }

    private void updatePath()
    {
        if (busChoice == busOptions.music1)
        {
            currentMusic = currentMusic.Replace("music2", "music1");
            transitionMusic = transitionMusic.Replace("music1", "music2");
        }
        else
        {
            currentMusic = currentMusic.Replace("music1", "music2");
            transitionMusic = transitionMusic.Replace("music2", "music1");
        }
    }
}
