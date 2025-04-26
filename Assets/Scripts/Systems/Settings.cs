using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Settings : Singleton<Settings>
{
    public DebugSettings Debug;

    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider DialogueSlider;

    private VolumeControl MasterVolume, MusicVolume, SFXVolume, DialogueVolume;

    void Start()
    {
        initializeVolumeSettings();
    }

    private void initializeVolumeSettings()
    {
        MasterVolume = transform.Find("MasterVolume").gameObject.AddComponent<VolumeControl>();
        MasterVolume.Initialize(MasterSlider, "bus:/");
        MasterVolume.StartControl();

        MusicVolume = transform.Find("MusicVolume").gameObject.AddComponent<VolumeControl>();
        MusicVolume.Initialize(MusicSlider, "bus:/Music");
        MusicVolume.StartControl();

        SFXVolume = transform.Find("SFXVolume").gameObject.AddComponent<VolumeControl>();
        SFXVolume.Initialize(SFXSlider, "bus:/SFX");
        SFXVolume.StartControl();

        DialogueVolume = transform.Find("DialogueVolume").gameObject.AddComponent<VolumeControl>();
        DialogueVolume.Initialize(DialogueSlider, "bus:/Dialogue");
        DialogueVolume.StartControl();
    }
}
