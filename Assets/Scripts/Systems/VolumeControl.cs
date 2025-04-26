using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using System;

public class VolumeControl : MonoBehaviour
{
    public Slider VolumeSlider;
    public String VolumeBusPath;
    private FMOD.Studio.Bus VolumeBus;

    public void StartControl()
    {
        // grab FMOD bus
        VolumeBus = RuntimeManager.GetBus(VolumeBusPath);

        float volume;
        if (false) // check if player preference exists
        {
            // TODO: Load Player Preferences into variables
        }
        else
        {
            // if does not exist, initialize slider based on parameter in FMOD
            VolumeBus.getVolume(out volume);
        }

        // TODO: Null ref, uncomment when slider added
        // VolumeSlider.value = volume;
    }

    public void ChangeVolume(float value)
    {
        if (VolumeBusPath.Substring(5) == "")
        {
            RuntimeManager.StudioSystem.setParameterByName("GlobalVolume", value);
        }
        else
        {
            RuntimeManager.StudioSystem.setParameterByName(VolumeBusPath.Substring(5) + "Volume", value);
        }

    }

    public void Initialize(Slider slider, String busPath)
    {
        VolumeSlider = slider;
        VolumeBusPath = busPath;
    }
}
