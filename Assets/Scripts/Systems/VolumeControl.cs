using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using System;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private Slider VolumeSlider;
    [SerializeField] private string VolumeBusPath;
    private FMOD.Studio.Bus VolumeBus;

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

    private void OnSliderChanged(float value)
    {
        VolumeBus.setVolume(value);
        VolumeBus.getVolume(out float volume);
        Debug.Log(VolumeBusPath + " new volume: " + Math.Round(volume, 5) * 100 + "%");
    }
}
