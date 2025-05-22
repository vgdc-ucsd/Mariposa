using FMOD.Studio;
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
    [SerializeField] private Slider AmbienceSlider;

    private VolumeControl MasterVolume, MusicVolume, SFXVolume, DialogueVolume, AmbienceVolume;

    void Start()
    {
        initializeVolumeSettings();

        // play test audio if audio debug is on to test volume slider functionality
        if (Debug.GetAudioDebug())
        {
            // test music (using mariposa's tutorial music)
            EventInstance audioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/music/s0_tutorial/mariposa");
            audioEvent.start();

            // test sfx (using bee flap sfx)
            audioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/sfx/player/bee/flap");
            audioEvent.start();

            // no dialogue exists yet so will add when it gets imported
        }
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

        AmbienceVolume = transform.Find("AmbienceVolume").gameObject.AddComponent<VolumeControl>();
        AmbienceVolume.Initialize(AmbienceSlider, "bus:/Ambience");
        AmbienceVolume.StartControl();
    }
}
