using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using UnityEngine.InputSystem;

/// <summary>
/// Unity moment
/// </summary>
public enum WindowType
{
    Windowed,
    Fullscreen,
    WindowedFullscreen
}

public class VideoSetting : MonoBehaviour
{

    private readonly int[,] resolutions = 
    {
        {640, 360},
        {1280, 720},
        {1600, 900},
        {1920, 1080},
        {2560, 1440},
        {3840, 2160}
    };
    public int currentResolutionIndex = 0;

    public int Width;
    public int Height;
    public WindowType ResolutionType = WindowType.Windowed;

    public InputActionReference EscapeInputAction;
    public GameObject PauseMenu;
    public GameObject VideoSettingsMenu;

    /// <summary>
    /// On program start, sets default resolution (may want to edit this if settings are saved)
    /// </summary>
    public void Start()
    {
        Width = resolutions[currentResolutionIndex,0];
        Height = resolutions[currentResolutionIndex,1];
        Screen.SetResolution(Width, Height, FullScreenMode.Windowed);
    }

    public void Update()
    {
        if (PauseMenu.activeInHierarchy) return; 
        
        if (EscapeInputAction.action.triggered)
        {
            Debug.Log("going back to pause menu from video settings");
            BackToPause();
        }   
    }

    /// <summary>
    /// Saves applied resolution to class when resolutionObj is clicked
    /// </summary>
    /// <param name="resolutionObj">the dropdown menu GameObject</param>
    public void SaveResolutionDimensions(GameObject resolutionObj)
    {
        currentResolutionIndex = resolutionObj.GetComponent<Dropdown>().value;
        Width = resolutions[currentResolutionIndex, 0];
        Height = resolutions[currentResolutionIndex, 1];
    }

    /// <summary>
    /// Saves resolution type to class when resolutionObj is clicked
    /// </summary>
    /// <param name="fullScreenObj">the dropdown menu GameObject</param>
    public void SaveResolutionType(GameObject fullscreenObj)
    {
        ResolutionType = (WindowType)fullscreenObj.GetComponent<Dropdown>().value;
    }

    /// <summary>
    /// Finally apply changes to resolution when resolutionObj is clicked
    /// </summary>
    public void ApplyResolutionChanges()
    {
        switch (ResolutionType)
        {
            case WindowType.Windowed: Screen.SetResolution(Width, Height, FullScreenMode.Windowed); return;
            case WindowType.Fullscreen: Screen.SetResolution(Width, Height, FullScreenMode.ExclusiveFullScreen); return;
            case WindowType.WindowedFullscreen: Screen.SetResolution(Width, Height, FullScreenMode.FullScreenWindow); return;
        }
    }

    public void BackToPause()
    {
        VideoSettingsMenu.SetActive(false);
        PauseMenu.SetActive(true);
    }
}
