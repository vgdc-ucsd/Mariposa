using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using System;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private Slider VolumeSlider;
    [SerializeField] private string VolumeBusPath;

    [Header("For Value display only. If slider null, allow adjustment.")]
    [SerializeField][Range(0f, 1f)] private float volume = 1f;
    private float previousVolume = 1f;
    private FMOD.Studio.Bus VolumeBus;

    private void Update()
    {
        // allow for inspector adjustments if volume slider does not exist
        if (VolumeSlider == null) { updateVolumeFromInspector(); }
    }

    public void StartControl()
    {
        // grab FMOD bus
        VolumeBus = RuntimeManager.GetBus(VolumeBusPath);

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
    public void Initialize(Slider slider, string busPath)
    {
        VolumeSlider = slider;
        VolumeBusPath = busPath;
    }

    private void updateVolumeFromInspector()
    {
        if (volume != previousVolume)
        {
            OnSliderChanged(volume);
            previousVolume = volume;
        }
    }

    private void OnSliderChanged(float value)
    {
        if (VolumeBus.isValid())
        {
            VolumeBus.setVolume(value);
            VolumeBus.getVolume(out volume);
        }
        else
        {
            Debug.LogError("Bus is not initialized!");
        }
        if (Settings.Instance.Debug.GetAudioDebug()) { Debug.Log(VolumeBusPath + " new volume: " + Math.Round(volume, 5) * 100 + "%"); }
    }
}
