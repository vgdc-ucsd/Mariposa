using System;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class Settings : Singleton<Settings>, IDataPersistence
{
    public DebugSettings Debug;
    [SerializeField] private AudioSetting audioSetting;

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
        if (audioSetting == null) audioSetting = FindFirstObjectByType<AudioSetting>();
        getSliders();
        // play background test audio if audio debug is on to test volume slider functionality
        // if (Debug.GetAudioDebug())
        // {
        //     backgroundAudioTest();
        // }
    }

    public void ResetVolumeSettings()
    {
        if (ResetAudioValues != null)
        {
            ResetAudioValues.Invoke();
            audioSetting.SetToDefaultVolume();
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

        initializeVolumeSettings();
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
        // TODO - Have to use keyword ref or out in order to reference, using FindFirstObjectyByType for now
        // if (this.audioSetting != null)
        // {
        //     UnityEngine.Debug.LogWarning("Audio setting referenced is already set! This may be caused by multiple instances of AudioSetting. Overriding current reference...");
        // }
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
        MasterVolume = MasterSlider.gameObject.AddComponent<VolumeControl>();
        MasterVolume.Initialize(MasterSlider, "bus:/");
        MasterVolume.StartControl();

        MusicVolume = MusicSlider.gameObject.AddComponent<VolumeControl>();
        MusicVolume.Initialize(MusicSlider, "bus:/Music", "event:/test/music");
        MusicVolume.StartControl();

        SFXVolume = SFXSlider.gameObject.AddComponent<VolumeControl>();
        SFXVolume.Initialize(SFXSlider, "bus:/SFX", "event:/test/sfx");
        SFXVolume.StartControl();

        DialogueVolume = DialogueSlider.gameObject.AddComponent<VolumeControl>();
        DialogueVolume.Initialize(DialogueSlider, "bus:/Dialogue", "event:/test/dialogue");
        DialogueVolume.StartControl();

        AmbienceVolume = AmbienceSlider.gameObject.AddComponent<VolumeControl>();
        AmbienceVolume.Initialize(AmbienceSlider, "bus:/Ambience", "event:/test/ambience");
        AmbienceVolume.StartControl();
    }

    public void PauseSounds(bool pause)
    {
        //MusicVolume?.pauseBus(pause);
        SFXVolume?.pauseBus(pause);     // TODO: there will be an issue with main menu sfx being muted
        DialogueVolume?.pauseBus(pause);
        AmbienceVolume?.pauseBus(pause);
    }

    public void MuteTestSounds(bool mute)
    {
        Bus testBus = RuntimeManager.GetBus("bus:/Test");
        testBus.setMute(mute);
    }
    
    public void SaveData(ref GameData data)
    {
        data.sfxValue = audioSetting.SfxSlider.value;
        data.musicValue = audioSetting.MusicSlider.value;
        data.masterValue = audioSetting.MasterSlider.value;
        data.ambienceValue = audioSetting.AmbienceSlider.value;
        data.dialogueValue = audioSetting.DialogueSlider.value;
    }
    public void LoadData(GameData data)
    {
        audioSetting.SfxSlider.value = data.sfxValue;
        audioSetting.MusicSlider.value = data.musicValue;
        audioSetting.MasterSlider.value = data.masterValue;
        audioSetting.AmbienceSlider.value = data.ambienceValue;
        audioSetting.DialogueSlider.value = data.dialogueValue;
    }
}