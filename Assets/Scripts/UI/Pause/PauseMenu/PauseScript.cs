using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseMenu;   
    public Slider sfxSlider;
    public Slider musicSlider;
    public Slider dialogueSlider;
    public Slider masterSlider;


    public InputActionReference pauseAction;
    public bool paused = false;

    public float sfxVolumeValue;
    public float musicVolumeValue;
    public float masterVolumeValue;
    public float dialogueVolumeValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1.0f;
        pauseAction.action.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseAction.action.triggered)
        {
            if (paused) 
            {
                Debug.Log("resume");
                ResumeGame();
            }
            else
            {
                Debug.Log("pause");
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
        paused = true;
    }


    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        paused = false;
    }

    public void UpdateSFXVolumeSetting()
    {
        Debug.Log("change detected");
        sfxVolumeValue = sfxSlider.value;
    }

    public void UpdateMusicVolumeSetting()
    {            
        Debug.Log("change detected");
        musicVolumeValue = musicSlider.value;
    }

    public void UpdateMasterVolumeSetting()
    {
        Debug.Log("change detected");
        masterVolumeValue = masterSlider.value;
        AudioListener.volume = sfxVolumeValue;
    }

    public void UpdateDialogueVolumeSetting()
    {
        Debug.Log("change detected");
        dialogueVolumeValue = dialogueSlider.value;
    }

    public void OpenVideoSettings(GameObject videoSettings)
    {
       // enable/open video settings gameobject
    }
}
