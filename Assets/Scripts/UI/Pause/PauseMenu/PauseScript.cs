using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseMenu;   
    public GameObject pauseMenuInstance;

    public InputActionReference pauseAction;
    public bool paused = false;

    public float sfxVolumeValue;
    public float musicVolumeValue;
    public float masterVolumeValue;
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

    public void UpdateSFXSetting(GameObject sfxSlider)
    {
        sfxVolumeValue = sfxSlider.GetComponent<Slider>().value;
    }

    public void UpdateMusicSetting(GameObject musicSlider)
    {
        sfxVolumeValue = musicSlider.GetComponent<Slider>().value;
    }

    public void UpdateMasterVolumeSetting(GameObject masterVolumeSlider)
    {
        sfxVolumeValue = masterVolumeSlider.GetComponent<Slider>().value;
        AudioListener.volume = sfxVolumeValue;
    }

    public void OpenVideoSettings(GameObject videoSettings)
    {
       // enable/open video settings gameobject
    }
}
