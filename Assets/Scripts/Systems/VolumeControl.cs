using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using System;
using FMOD.Studio;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private Slider VolumeSlider;
    [SerializeField] private string VolumeBusPath;
    [SerializeField] private string testAudioPath;

    private float volume = 1f;
    private float previousVolume = 1f;
    private Bus VolumeBus;
    private EventInstance testAudio;
    [SerializeField] private float testAudioTimer = -1.0f;
    private bool isTestAudioPlaying = false;
    PLAYBACK_STATE testAudioState;

    void Update()
    {
        if (testAudioTimer >= 0.0f)
        {
            isTestAudioPlaying = true;
            testAudioTimer -= Time.unscaledDeltaTime;
        }
        else
        {
            isTestAudioPlaying = false;
        }
        updateTestAudio(isTestAudioPlaying);
    }

    public void StartControl()
    {
        // grab FMOD bus
        VolumeBus = RuntimeManager.GetBus(VolumeBusPath);
        Settings.Instance.ChangeTestAudio.AddListener(changeTestAudio);
        if (testAudioPath != null) { testAudio = RuntimeManager.CreateInstance(testAudioPath); }

        if (false) // check if player preference exists
        {
            // TODO: Load Player Preferences into variables
        }
        else
        {
            // if no player pref found, set volume to 100%
            VolumeBus.setVolume(1.0f);
        }

        // calls OnSliderChanged with new value whenever the slider changes
        VolumeSlider?.onValueChanged.AddListener(OnSliderChanged);
    }
    public void Initialize(Slider slider, string busPath, string testAudioPath = null)
    {
        VolumeSlider = slider;
        VolumeBusPath = busPath;
        this.testAudioPath = testAudioPath;
    }

    private void OnSliderChanged(float value)
    {
        if (VolumeBus.isValid())
        {
            VolumeBus.setVolume(value);
            VolumeBus.getVolume(out volume);
            playSliderClick();
            Settings.Instance.ChangeTestAudio?.Invoke(VolumeBusPath);
            if (testAudio.isValid()) { testAudio.setVolume(volume); }
        }
        else
        {
            Debug.LogError("Bus is not initialized!");
        }
        //if (Settings.Instance.Debug.GetAudioDebug()) { Debug.Log(VolumeBusPath + " new volume: " + Math.Round(volume, 5) * 100 + "%"); }
    }

    private void playSliderClick()
    {
        if (Math.Abs(Mathf.Floor(volume * 100) - Mathf.Floor(previousVolume * 100)) >= 4)
        {
            RuntimeManager.PlayOneShot("event:/test/slider_click");
            previousVolume = volume;
        }
    }

    private void updateTestAudio(bool play)
    {
        testAudio.getPlaybackState(out testAudioState);
        if (play)
        {
            playTestAudio();
        }
        else
        {
            stopTestAudio();
        }
    }

    private void changeTestAudio(string invokerBusName)
    {
        if (invokerBusName.Equals("bus:/"))
        {
            testAudioTimer = 3.0f;
        }
        else if (invokerBusName.Equals(VolumeBusPath))
        {
            testAudioTimer = 3.0f;
        }
        else
        {
            testAudioTimer = -1.0f;
        }
    }

    private void playTestAudio()
    {
        if (testAudioState != PLAYBACK_STATE.PLAYING)
        {
            testAudio.start();
        }
    }

    private void stopTestAudio()
    {
        if ((testAudioState != PLAYBACK_STATE.STOPPING) && (testAudioState != PLAYBACK_STATE.STOPPED))
        {
            testAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void pauseBus(bool pause)
    {
        VolumeBus.setPaused(pause);
    }
}
