using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using System;
using FMOD.Studio;
using UnityEngine.EventSystems;

public class VolumeControl : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private Slider VolumeSlider;
    [SerializeField] private string VolumeBusPath;
    [SerializeField] private string testAudioPath;

    private float volume = 1f;
    private float previousVolume = 1f;
    private Bus VolumeBus;
    private EventInstance testAudio;
    [SerializeField] private float testAudioTimer = -1.0f;
    private const float TEST_AUDIO_AFK_DURATION = 2.0f;
    private bool isTestAudioPlaying = false;
    private bool isResetting = false;
    private bool needsSaving = false;
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
        Settings.Instance.ResetAudioValues += resetVolume;
        if (testAudioPath != null) { testAudio = RuntimeManager.CreateInstance(testAudioPath); }

        if (VolumeSlider == null)
        {
            Debug.LogError($"No slider detected for {VolumeBusPath}! Failed to start volume control!");
            return;
        }

        // check if player preference exists and updates slider value and internal values
        volume = PlayerPrefs.GetFloat(VolumeBusPath, 1.0f);
        previousVolume = volume;
        VolumeSlider.value = volume;

        // calls OnSliderChanged with new value whenever the slider changes
        VolumeSlider.onValueChanged.AddListener(OnSliderChanged);
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
            needsSaving = true;
            updateVolume(value);

            if (isResetting)
            {
                previousVolume = value;
                saveVolumeToPlayerPrefs();
                isResetting = false;
            }
            else
            {
                playSliderClick();
                if (testAudio.isValid()) { testAudio.setVolume(volume); }
                Settings.Instance.ChangeTestAudio?.Invoke(VolumeBusPath);
            }
        }
        else
        {
            Debug.LogError("Bus is not initialized!");
        }
    }

    private void updateVolume(float value)
    {
        VolumeBus.setVolume(value);
        VolumeBus.getVolume(out volume);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (needsSaving)
        {
            saveVolumeToPlayerPrefs();
            needsSaving = false;
        }
    }

    private void saveVolumeToPlayerPrefs()
    {
        PlayerPrefs.SetFloat(VolumeBusPath, volume);
        PlayerPrefs.Save();
    }

    private void playSliderClick()
    {
        if (Math.Abs(Mathf.Floor(volume * 100) - Mathf.Floor(previousVolume * 100)) >= 4)
        {
            EventInstance slider_click = RuntimeManager.CreateInstance("event:/test/slider_click");
            slider_click.setVolume(Settings.Instance.internalUsageGetSFXVolume());
            slider_click.start();
            slider_click.release();
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
            testAudioTimer = TEST_AUDIO_AFK_DURATION;
        }
        else if (invokerBusName.Equals(VolumeBusPath))
        {
            testAudioTimer = TEST_AUDIO_AFK_DURATION;
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
            testAudio.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    public void pauseBus(bool pause)
    {
        VolumeBus.setPaused(pause);
    }

    private void resetVolume()
    {
        isResetting = true;
        testAudioTimer = -1.0f;
    }
}