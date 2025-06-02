using System;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class Settings : Singleton<Settings>
{
    public DebugSettings Debug;
    private AudioSetting audioSetting;

    private Slider MasterSlider;
    private Slider MusicSlider;
    private Slider SFXSlider;
    private Slider DialogueSlider;
    private Slider AmbienceSlider;

    public UnityEvent<string> ChangeTestAudio;
    public UnityAction ResetAudioValues;

    private VolumeControl MasterVolume, MusicVolume, SFXVolume, DialogueVolume, AmbienceVolume;

    private void Start()
    {
        getSliders();
        initializeVolumeSettings();
        // play background test audio if audio debug is on to test volume slider functionality
        if (Debug.GetAudioDebug())
        {
            backgroundAudioTest();
        }
    }

    public void ResetVolumeSettings()
    {
        if (ResetAudioValues != null)
        {
            ResetAudioValues.Invoke();
            audioSetting.ResetVolumeSettings();
        }
        else
        {
            UnityEngine.Debug.LogWarning("No VolumeControls attached to volume settings. Unable to invoke!");
        }
    }

    private void getSliders()
    {
        if (this.audioSetting == null)
        {
            UnityEngine.Debug.LogWarning("No AudioSetting reference found! Attempting to use null references.");
            return;
        }

        MasterSlider = audioSetting.MasterSlider;
        checkSliderImport(MasterSlider);
        MusicSlider = audioSetting.MusicSlider;
        checkSliderImport(MusicSlider);
        SFXSlider = audioSetting.SfxSlider;
        checkSliderImport(SFXSlider);
        DialogueSlider = audioSetting.DialogueSlider;
        checkSliderImport(DialogueSlider);
        AmbienceSlider = audioSetting.AmbienceSlider;
        checkSliderImport(AmbienceSlider);
    }

    public float internalUsageGetSFXVolume()
    {
        return SFXSlider.value;
    }

    private void checkSliderImport(Slider slider)
    {
        if (slider == null)
        {
            UnityEngine.Debug.LogWarning("No volume slider found! Attempting to use null reference.");
        }
    }

    public void setAudioSettingReference(AudioSetting audioSetting)
    {
        if (this.audioSetting != null)
        {
            UnityEngine.Debug.LogWarning("Audio setting referenced is already set! This may be caused by multiple instances of AudioSetting. Overriding current reference...");
        }
        this.audioSetting = audioSetting;
    }

    private void backgroundAudioTest()
    {
        // test music (using mariposa's tutorial music)
        EventInstance audioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/music1/s0_subway_tutorial/theme_mariposa");
        audioEvent.start();

        // test ambience
        audioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/ambience/s0_subway_tutorial/mariposa");
        audioEvent.start();

        // test sfx (using bee flap sfx)
        audioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/sfx/player/bee/flap");
        audioEvent.start();

        // test dialogue
        audioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/dialogue/luke/npc_greetings/what_can_i_do_for_you1");
        audioEvent.start();
    }

    private void initializeVolumeSettings()
    {
        MasterVolume = transform.Find("MasterVolume").gameObject.AddComponent<VolumeControl>();
        MasterVolume.Initialize(MasterSlider, "bus:/");
        MasterVolume.StartControl();

        MusicVolume = transform.Find("MusicVolume").gameObject.AddComponent<VolumeControl>();
        MusicVolume.Initialize(MusicSlider, "bus:/Music", "event:/test/music");
        MusicVolume.StartControl();

        SFXVolume = transform.Find("SFXVolume").gameObject.AddComponent<VolumeControl>();
        SFXVolume.Initialize(SFXSlider, "bus:/SFX", "event:/test/sfx");
        SFXVolume.StartControl();

        DialogueVolume = transform.Find("DialogueVolume").gameObject.AddComponent<VolumeControl>();
        DialogueVolume.Initialize(DialogueSlider, "bus:/Dialogue", "event:/test/dialogue");
        DialogueVolume.StartControl();

        AmbienceVolume = transform.Find("AmbienceVolume").gameObject.AddComponent<VolumeControl>();
        AmbienceVolume.Initialize(AmbienceSlider, "bus:/Ambience", "event:/test/ambience");
        AmbienceVolume.StartControl();
    }

    public void PauseSounds(bool pause)
    {
        MusicVolume?.pauseBus(pause);
        SFXVolume?.pauseBus(pause);     // TODO: there will be an issue with main menu sfx being muted
        DialogueVolume?.pauseBus(pause);
        AmbienceVolume?.pauseBus(pause);
    }

    public void MuteTestSounds(bool mute)
    {
        Bus testBus = RuntimeManager.GetBus("bus:/Test");
        testBus.setMute(mute);
    }
}