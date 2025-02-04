using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ResolutionSetting : MonoBehaviour
{

    private int[,] resolutions = {{640, 360}, {1280, 720}, {1600, 900}, {1920, 1080}, {2560, 1440}, {3840, 2160}};
    public int currentResolutionIndex = 0;


    public int Width;
    public int Height;
    public bool Fullscreen = true;

    /// <summary>
    /// On program start, sets default resolution (may want to edit this if settings are saved)
    /// </summary>
    public void Start()
    {
        Width = resolutions[currentResolutionIndex,0];
        Height = resolutions[currentResolutionIndex,1];
        Screen.SetResolution(Width, Height, false);
    }

    /// <summary>
    /// Changes resolution when button to apply changes is clicked
    /// </summary>
    /// <param name="dropdown">the dropdown menu GameObject</param>
    public void SetRes(GameObject dropdown)
    {
        if (Fullscreen)
        {
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, Screen.fullScreen);
            return;
        }

        currentResolutionIndex = dropdown.GetComponent<Dropdown>().value;
        Width = resolutions[currentResolutionIndex, 0];
        Height = resolutions[currentResolutionIndex, 1];
        Screen.SetResolution(Width, Height, false);
        Debug.Log(Screen.currentResolution);
    }

    /// <summary>
    /// Updates Fullscreen field when checkbox is changed
    /// </summary>
    /// <param name="checkbox">the toggle fulscreen checkbox GameObject</param>
    public void UpdateSettingsOnFullscreenToggle(GameObject checkbox)
    {
        Fullscreen = checkbox.GetComponent<Toggle>().isOn;
    }

    /// <summary>
    /// changes interactibility of resolutions dropdown menu, called right after UpdateSettingsOnFullscreenToggle
    /// </summary>
    /// <param name="dropdown">the dropdown menu GameObject</param>
    public void ResolutionReaction(GameObject dropdown)
    {
        dropdown.GetComponent<Dropdown>().interactable = !Fullscreen;  
    }


}
