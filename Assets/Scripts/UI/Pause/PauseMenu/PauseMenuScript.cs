using UnityEngine;

public class PauseMenuScript : Singleton<PauseMenuScript>
{
    public GameObject PauseMenu;
    public GameObject VideoSettingsMenu;
    public GameObject AudioSettingsMenu;
    [SerializeField] private UnityEngine.UI.Button checkpointButton;

    void Start()
    {
        GameManager.Instance.RegisterStartAction(GameState.PAUSE, PauseGame);
        GameManager.Instance.RegisterExitAction(GameState.PAUSE, ResumeGame);
    }

    public void PauseGame()
    {
        OpenPauseMenu();
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        CloseAllMenus();
        Time.timeScale = 1.0f;
    }

    public void OpenVideoSettings()
    {
        PauseMenu.SetActive(false);
        AudioSettingsMenu.SetActive(false);
        VideoSettingsMenu.SetActive(true);
        Settings.Instance.PauseSounds(true);
        Settings.Instance.MuteTestSounds(true);
    }

    public void OpenAudioSettings()
    {
        PauseMenu.SetActive(false);
        AudioSettingsMenu.SetActive(true);
        VideoSettingsMenu.SetActive(false);
        Settings.Instance.PauseSounds(true);
        Settings.Instance.MuteTestSounds(false);
    }

    public void OpenPauseMenu()
    {
        PauseMenu.SetActive(true);
        AudioSettingsMenu.SetActive(false);
        VideoSettingsMenu.SetActive(false);
        Settings.Instance.PauseSounds(true);
        Settings.Instance.MuteTestSounds(true);
        bool canCheckpoint = !(DialogueManager.Instance != null && DialogueManager.Instance.isPlayingDialogue)
                             && !(PuzzlePopupManager.Instance != null && PuzzlePopupManager.Instance.ActivePuzzle != null);
        if (checkpointButton != null)
            checkpointButton.interactable = canCheckpoint;
    }

    public void CloseAllMenus()
    {
        PauseMenu.SetActive(false);
        AudioSettingsMenu.SetActive(false);
        VideoSettingsMenu.SetActive(false);
        Settings.Instance.PauseSounds(false);
        Settings.Instance.MuteTestSounds(true);
    }

    public void GoToMainMenu()
    {
        CloseAllMenus();
        ResumeGame();
        GameManager.Instance.LoadScene(GameScene.MAIN_MENU);
    }

    public void RestartFromCheckpoint()
    {
        if (DialogueManager.Instance != null || PuzzlePopupManager.Instance != null)
        {
            return;
        }
        CloseAllMenus();
        ResumeGame();
        // respawn from level manager or something?
    }

    public void RestartLevel()
    {
        ResumeGame();
        CloseAllMenus();
        if (LevelManager.Instance == null)
        {
            Debug.LogWarning("Pause manager attempting to restart level but LevelManager not found!");
            return;
        }
        LevelManager.Instance.RestartLevel();
    }

    public void QuitLevel()
    {
        Time.timeScale = 1.0f;
        GameManager.Instance.LoadScene(GameScene.MAIN_MENU);
    }
}
