using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject VideoSettingsMenu;
    public GameObject AudioSettingsMenu;

    private InputSystem_Actions actions;
    public bool paused = false;
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
        if (!paused)
        {
            PauseGame();
            return;
        }

        if (PauseMenu.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            OpenPauseMenu();
        }
    }

    // Update is called once per frame
    void Update()
    {
        return;
    }

    public void PauseGame()
    {
        OpenPauseMenu();
        Time.timeScale = 0.0f;
        paused = true;
    }


    public void ResumeGame()
    {
        CloseAllMenus();
        Time.timeScale = 1.0f;
        paused = false;
    }


    public void OpenVideoSettings()
    {
        PauseMenu.SetActive(false);
        AudioSettingsMenu.SetActive(false);
        VideoSettingsMenu.SetActive(true);
    }

    public void OpenAudioSettings()
    {
        PauseMenu.SetActive(false);
        AudioSettingsMenu.SetActive(true);
        VideoSettingsMenu.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        PauseMenu.SetActive(true);
        AudioSettingsMenu.SetActive(false);
        VideoSettingsMenu.SetActive(false);
    }

    public void CloseAllMenus()
    {
        PauseMenu.SetActive(false);
        AudioSettingsMenu.SetActive(false);
        VideoSettingsMenu.SetActive(false);
    }
}
