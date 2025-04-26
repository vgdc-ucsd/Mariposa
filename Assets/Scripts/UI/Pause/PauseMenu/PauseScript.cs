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


    public InputActionReference pauseAction;
    public bool paused = false;

    public float sfxVolumeValue;
    public float musicVolumeValue;
    public float masterVolumeValue;
    public float dialogueVolumeValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VideoSettingsMenu.SetActive(false);
        PauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        pauseAction.action.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseAction.action.triggered && !VideoSettingsMenu.activeSelf)
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

    public void UpdateSFXVolumeSetting()
    {
        Debug.Log("change detected");
        sfxVolumeValue = SfxSlider.value;
    }

    public void UpdateMusicVolumeSetting()
    {            
        Debug.Log("change detected");
        musicVolumeValue = MusicSlider.value;
    }

    public void UpdateMasterVolumeSetting()
    {
        Debug.Log("change detected");
        masterVolumeValue = MasterSlider.value;
        AudioListener.volume = sfxVolumeValue;
    }

    public void UpdateDialogueVolumeSetting()
    {
        Debug.Log("change detected");
        dialogueVolumeValue = DialogueSlider.value;
    }

    public void OpenVideoSettings()
    {
        Debug.Log("setting video settings");
        PauseMenu.SetActive(false);
        VideoSettingsMenu.SetActive(true);
    }
}
