using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    [Header("Refer to Settings for Volume Settings and Controls")]
    public Slider SfxSlider;
    public Slider MusicSlider;
    public Slider DialogueSlider;
    public Slider MasterSlider;
    public Slider AmbienceSlider;

    [SerializeField] private float defaultSFXVolumeValue;
    [SerializeField] private float defaultMusicVolumeValue;
    [SerializeField] private float defaultMasterVolumeValue;
    [SerializeField] private float defaultDialogueVolumeValue;
    [SerializeField] private float defaultAmbienceVolumeValue;

    void Start()
    {
        // sending reference to Settings to handle audio controls
        Settings.Instance.setAudioSettingReference(this);
    }

    // Note: The gameObject "Settings" is directly grabbing the slider values to update volume, only default values are stored here

    public void ResetVolumeSettings()
    {
        SfxSlider.value = defaultSFXVolumeValue;
        MusicSlider.value = defaultMusicVolumeValue;
        MasterSlider.value = defaultMasterVolumeValue;
        AmbienceSlider.value = defaultAmbienceVolumeValue;
        DialogueSlider.value = defaultDialogueVolumeValue;
    }
}