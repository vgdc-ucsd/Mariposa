using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    public Slider SfxSlider;
    public Slider MusicSlider;
    public Slider DialogueSlider;
    public Slider MasterSlider;
    public Slider AmbienceSlider;

    
    public float sfxVolumeValue;
    public float musicVolumeValue;
    public float masterVolumeValue;
    public float dialogueVolumeValue;
    public float ambienceVolumeValue;

    [SerializeField] private float defaultSFXVolumeValue;
    [SerializeField] private float defaultMusicVolumeValue;
    [SerializeField] private float defaultMasterVolumeValue;
    [SerializeField] private float defaultDialogueVolumeValue;
    [SerializeField] private float defaultAmbienceVolumeValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    // Note: The gameObject "Settings" is directly grabbing the slider values to update volume
    // the update methods below were kept here in-case the developer wants to still have access to the values
    public void UpdateSFXVolumeSetting()
    {
        if (Settings.Instance.Debug.GetAudioDebug()) { Debug.Log("change detected"); }
        sfxVolumeValue = SfxSlider.value;
    }
    public void UpdateMusicVolumeSetting()
    {
        if (Settings.Instance.Debug.GetAudioDebug()) { Debug.Log("change detected"); }
        musicVolumeValue = MusicSlider.value;
    }

    public void UpdateAmbienceVolumeSetting()
    {
        if (Settings.Instance.Debug.GetAudioDebug()) { Debug.Log("change detected"); }
        ambienceVolumeValue = AmbienceSlider.value;
    }

    public void UpdateMasterVolumeSetting()
    {
        if (Settings.Instance.Debug.GetAudioDebug()) { Debug.Log("change detected"); }
        masterVolumeValue = MasterSlider.value;
        AudioListener.volume = sfxVolumeValue;
    }

    public void UpdateDialogueVolumeSetting()
    {
        if (Settings.Instance.Debug.GetAudioDebug()) { Debug.Log("change detected"); }
        dialogueVolumeValue = DialogueSlider.value;
    }

    public void ResetVolumeSettings()
    {
        SfxSlider.value = defaultSFXVolumeValue;
        sfxVolumeValue = defaultSFXVolumeValue;
        MusicSlider.value = defaultMusicVolumeValue;
        musicVolumeValue = defaultMusicVolumeValue;
        MasterSlider.value = defaultMasterVolumeValue;
        masterVolumeValue = defaultMasterVolumeValue;
        AmbienceSlider.value = defaultAmbienceVolumeValue;
        ambienceVolumeValue = defaultAmbienceVolumeValue;
        DialogueSlider.value = defaultDialogueVolumeValue;
        dialogueVolumeValue = defaultDialogueVolumeValue;
    }
}
