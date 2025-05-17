using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

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

    public RenderPipelineAsset[] QualityLevels;

    private readonly int[,] resolutions =
    {
        {640, 360},
        {1280, 720},
        {1600, 900},
        {1920, 1080},
        {2560, 1440},
        {3840, 2160}
    };
    public int graphicsQualityIndex = 2;
    public WindowType ResolutionType = WindowType.Windowed;
    public int currentResolutionIndex = 5;

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
        Width = resolutions[currentResolutionIndex, 0];
        Height = resolutions[currentResolutionIndex, 1];
        Screen.SetResolution(Width, Height, FullScreenMode.Windowed);

        GraphicsQualityDropdown.value = QualitySettings.GetQualityLevel();
        graphicsQualityIndex = GraphicsQualityDropdown.value;
    }

    /// <summary>
    /// Saves applied resolution to class when resolutionObj is clicked
    /// </summary>
    /// <param name="resolutionObj">the dropdown menu GameObject</param>
    public void ApplyResolutionDimensionsChanges()
    {
        currentResolutionIndex = ResolutionSizeDropdown.value;
        Width = resolutions[currentResolutionIndex, 0];
        Height = resolutions[currentResolutionIndex, 1];
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
        Debug.Log("caca");
        GraphicsQualityDropdown.value = defaultQualityIndex;
        Debug.Log(GraphicsQualityDropdown.value);
        ResolutionTypeDropdown.value = defaultResolutionTypeIndex;
        ResolutionSizeDropdown.value = defaultResolutionIndex;

        ApplyAllGraphicsChanges();
        Debug.Log("loco");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.U))
        {
            ResolutionSizeDropdown.value++;
            ApplyAllGraphicsChanges();
        }
    }
}
