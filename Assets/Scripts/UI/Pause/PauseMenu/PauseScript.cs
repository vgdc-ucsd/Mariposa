using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    public GameObject PauseMenu;
    public Slider SfxSlider;
    public Slider MusicSlider;
    public Slider DialogueSlider;
    public Slider MasterSlider;

    public GameObject VideoSettingsMenu;


    private InputSystem_Actions actions;
    public bool paused = false;

    public float sfxVolumeValue;
    public float musicVolumeValue;
    public float masterVolumeValue;
    public float dialogueVolumeValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        actions = new();
    }

    // void Start()
    // {
    //     VideoSettingsMenu.SetActive(false);
    //     PauseMenu.SetActive(false);
    //     Time.timeScale = 1.0f;
    // }

    void OnEnable()
    {
        actions.Player.Pause.Enable();
        actions.Player.Pause.started += HandlePause;
    }

    void OnDisable()
    {
        actions.Player.Pause.started -= HandlePause;
        actions.Player.Pause.Disable();
    }

    private void HandlePause(InputAction.CallbackContext ctx)
    {
        if (!VideoSettingsMenu.activeSelf)
        {
            if (paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        return;
    }

    public void PauseGame()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
        paused = true;
    }


    public void ResumeGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        paused = false;
    }

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

    public void OpenVideoSettings()
    {
        Debug.Log("setting video settings");
        PauseMenu.SetActive(false);
        VideoSettingsMenu.SetActive(true);
    }
}
