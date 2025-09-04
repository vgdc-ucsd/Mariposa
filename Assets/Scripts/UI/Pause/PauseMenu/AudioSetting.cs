using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    public Slider MasterSlider;
    public Slider MusicSlider;
    public Slider SfxSlider;
    public Slider AmbienceSlider;
    public Slider DialogueSlider;

    private void OnEnable()
    {
        MasterSlider.value = AudioManager.Instance.masterVolume;
        MusicSlider.value = AudioManager.Instance.musicVolume;
        SfxSlider.value = AudioManager.Instance.sfxVolume;
        AmbienceSlider.value = AudioManager.Instance.ambienceVolume;
        DialogueSlider.value = AudioManager.Instance.dialogueVolume;
    }

    public void UpdateMasterVolume() => AudioManager.Instance.SetMasterVolume(MasterSlider.value);
    public void UpdateMusicVolume() => AudioManager.Instance.SetMusicVolume(MusicSlider.value);
    public void UpdateSFXVolume() => AudioManager.Instance.SetSFXVolume(SfxSlider.value);
    public void UpdateAmbienceVolume() => AudioManager.Instance.SetAmbienceVolume(AmbienceSlider.value);
    public void UpdateDialogueVolume() => AudioManager.Instance.SetDialogueVolume(DialogueSlider.value);

    public void ResetAllVolumesToDefault()
    {
        MasterSlider.value = AudioManager.Instance.defaultMasterVolume;
        MusicSlider.value = AudioManager.Instance.defaultMusicVolume;
        SfxSlider.value = AudioManager.Instance.defaultSFXVolume;
        AmbienceSlider.value = AudioManager.Instance.defaultAmbienceVolume;
        DialogueSlider.value = AudioManager.Instance.defaultDialogueVolume;

        UpdateMasterVolume();
        UpdateMusicVolume();
        UpdateSFXVolume();
        UpdateAmbienceVolume();
        UpdateDialogueVolume();
    }
}