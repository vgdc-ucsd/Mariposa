using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Settings : Singleton<Settings>
{
    public DebugSettings Debug;

    [SerializeField]
    public Slider MasterSlider;
    [SerializeField]
    public Slider MusicSlider;
    [SerializeField]
    public Slider SFXSlider;
    [SerializeField]
    public Slider DialogueSlider;

    VolumeControl MasterVolume, MusicVolume, SFXVolume, DialogueVolume;

    void Start()
    {
        MasterVolume = gameObject.AddComponent<VolumeControl>();
        MasterVolume.Initialize(MasterSlider, "bus:/");
        MasterVolume.StartControl();

        MusicVolume = gameObject.AddComponent<VolumeControl>();
        MusicVolume.Initialize(MusicSlider, "bus:/Music");
        MusicVolume.StartControl();

        SFXVolume = gameObject.AddComponent<VolumeControl>();
        SFXVolume.Initialize(SFXSlider, "bus:/SFX");
        SFXVolume.StartControl();

        DialogueVolume = gameObject.AddComponent<VolumeControl>();
        DialogueVolume.Initialize(DialogueSlider, "bus:/Dialogue");
        DialogueVolume.StartControl();

        MasterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
        MusicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        SFXSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        DialogueSlider.onValueChanged.AddListener(OnDialogueSliderChanged);
    }

    public void OnMasterSliderChanged(float value)
    {
        MasterVolume.ChangeVolume(value);
    }

    public void OnMusicSliderChanged(float value)
    {
        MusicVolume.ChangeVolume(value);
    }

    public void OnSFXSliderChanged(float value)
    {
        SFXVolume.ChangeVolume(value);
    }

    public void OnDialogueSliderChanged(float value)
    {
        DialogueVolume.ChangeVolume(value);
    }

    private void OnDestroy()
    {
        // Remove listener to prevent memory leaks or unexpected behavior
        MasterSlider.onValueChanged.RemoveListener(OnMasterSliderChanged);
        MusicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        SFXSlider.onValueChanged.RemoveListener(OnSFXSliderChanged);
        DialogueSlider.onValueChanged.RemoveListener(OnDialogueSliderChanged);
    }
}
