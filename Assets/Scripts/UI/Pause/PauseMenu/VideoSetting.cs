using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Unity moment
/// </summary>
public enum WindowType
{
    Windowed,
    Fullscreen,
    WindowedFullscreen
}

public class VideoSetting : MonoBehaviour, IDataPersistence
{

    public RenderPipelineAsset[] QualityLevels;

    public int graphicsQualityIndex = 2;
    public WindowType ResolutionType = WindowType.WindowedFullscreen;
    public int currentResolutionIndex;

    private int[,] resolutions =
    {
        {640, 360},
        {1280, 720},
        {1600, 900},
        {1920, 1080},
        {2560, 1440},
        {3840, 2160}
    };
    public int Width;
    public int Height;

    public GameObject PauseMenu;
    public GameObject VideoSettingsMenu;

    [SerializeField] private Dropdown GraphicsQualityDropdown;
    [SerializeField] private Dropdown ResolutionTypeDropdown;
    [SerializeField] private Dropdown ResolutionSizeDropdown;

    [SerializeField] private int defaultQualityIndex;
    [SerializeField] private int defaultResolutionTypeIndex;
    [SerializeField] private int defaultResolutionIndex;

    /// <summary>
    /// On program start, sets default resolution (may want to edit this if settings are saved)
    /// </summary>
    public void Start()
    {
        ResolutionSizeDropdown.ClearOptions();
        ResolutionSizeDropdown.AddOptions(Screen.resolutions.Select(x => x.ToString()).ToList());
        currentResolutionIndex = Screen.resolutions.Length - 1;

        Width = Screen.resolutions[currentResolutionIndex].width;
        Height = Screen.resolutions[currentResolutionIndex].height;
        Screen.SetResolution(Width, Height, FullScreenMode.FullScreenWindow);

        ResolutionSizeDropdown.value = currentResolutionIndex;
        GraphicsQualityDropdown.value = QualitySettings.GetQualityLevel();
        ResolutionTypeDropdown.value = 2;
        graphicsQualityIndex = GraphicsQualityDropdown.value;
    }

    /// <summary>
    /// Saves applied resolution to class when resolutionObj is clicked
    /// </summary>
    /// <param name="resolutionObj">the dropdown menu GameObject</param>
    public void ApplyResolutionDimensionsChanges()
    {
        currentResolutionIndex = ResolutionSizeDropdown.value;
        Width = Screen.resolutions[currentResolutionIndex].width;
        Height = Screen.resolutions[currentResolutionIndex].height;
    }

    /// <summary>
    /// Finally apply changes to resolution when resolutionObj is clicked
    /// </summary>
    public void ApplyResolutionTypeChanges()
    {
        ResolutionType = (WindowType)ResolutionTypeDropdown.value;
        switch (ResolutionType)
        {
            case WindowType.Windowed: Screen.SetResolution(Width, Height, FullScreenMode.Windowed); return;
            case WindowType.Fullscreen: Screen.SetResolution(Width, Height, FullScreenMode.ExclusiveFullScreen); return;
            case WindowType.WindowedFullscreen: Screen.SetResolution(Width, Height, FullScreenMode.FullScreenWindow); return;
        }
    }

    public void ChangeGraphicsQuality()
    {
        graphicsQualityIndex = GraphicsQualityDropdown.value;
        QualitySettings.SetQualityLevel(graphicsQualityIndex);
        QualitySettings.renderPipeline = QualityLevels[graphicsQualityIndex];
    }

    public void ApplyAllGraphicsChanges()
    {
        ChangeGraphicsQuality();
        ApplyResolutionDimensionsChanges();
        ApplyResolutionTypeChanges();
    }

    public void ResetGraphicsSettings()
    {
        GraphicsQualityDropdown.value = defaultQualityIndex;
        ResolutionTypeDropdown.value = defaultResolutionTypeIndex;
        ResolutionSizeDropdown.value = defaultResolutionIndex;
        ApplyAllGraphicsChanges();
    }

    public void SaveData(ref GameData data)
    {
        data.height = Height;
        data.width = Width;
        data.windowType = ResolutionType;
    }
    public void LoadData(GameData data)
    {
        Height = data.height;
        Width = data.width;
        ResolutionType = data.windowType;
        ApplyAllGraphicsChanges();
    }
}