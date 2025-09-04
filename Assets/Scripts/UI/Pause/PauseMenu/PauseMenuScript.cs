using UnityEngine;

public class PauseMenuScript : Singleton<PauseMenuScript>
{
    public GameObject PauseMenu;
    public GameObject VideoSettingsMenu;
    public GameObject AudioSettingsMenu;
    public GameObject RestartConfirmMenu;
    public GameObject ExitConfirmMenu;
    public GameObject BackgroundImage;

    void Start()
    {
        GameManager.Instance.RegisterStartAction(GameState.PAUSE, PauseGame);
        GameManager.Instance.RegisterExitAction(GameState.PAUSE, ResumeGame);
    }

    public void ResumeGameButton()
    {
        GameManager.Instance.HandlePause();
    }

    public void PauseGame()
    {
        OpenPauseMenu();
        BackgroundImage.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        CloseAllMenus();
        BackgroundImage.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void OpenVideoSettings()
    {
        PauseMenu.SetActive(false);
        AudioSettingsMenu.SetActive(false);
        VideoSettingsMenu.SetActive(true);
        RestartConfirmMenu.SetActive(false);
        ExitConfirmMenu.SetActive(false);
        Settings.Instance.PauseSounds(true);
        Settings.Instance.MuteTestSounds(true);
    }

    public void OpenAudioSettings()
    {
        PauseMenu.SetActive(false);
        AudioSettingsMenu.SetActive(true);
        VideoSettingsMenu.SetActive(false);
        RestartConfirmMenu.SetActive(false);
        ExitConfirmMenu.SetActive(false);
        Settings.Instance.PauseSounds(true);
        Settings.Instance.MuteTestSounds(false);
    }

    public void OpenPauseMenu()
    {
        GameScene currentScene = GameManager.Instance.CurrentScene;
        bool reopenMenu = currentScene != GameScene.MAIN_MENU && currentScene != GameScene.CREDITS;

        PauseMenu.SetActive(reopenMenu);
        AudioSettingsMenu.SetActive(false);
        VideoSettingsMenu.SetActive(false);
        RestartConfirmMenu.SetActive(false);
        ExitConfirmMenu.SetActive(false);
        Settings.Instance.PauseSounds(true);
        Settings.Instance.MuteTestSounds(true);
    }

    public void OpenRestartConfirmMenu()
    {
        PauseMenu.SetActive(false);
        AudioSettingsMenu.SetActive(false);
        VideoSettingsMenu.SetActive(false);
        RestartConfirmMenu.SetActive(true);
        ExitConfirmMenu.SetActive(false);
        Settings.Instance.PauseSounds(true);
        Settings.Instance.MuteTestSounds(true);
    }

    public void OpenExitConfirmMenu()
    {
        PauseMenu.SetActive(false);
        AudioSettingsMenu.SetActive(false);
        VideoSettingsMenu.SetActive(false);
        RestartConfirmMenu.SetActive(false);
        ExitConfirmMenu.SetActive(true);
        Settings.Instance.PauseSounds(true);
        Settings.Instance.MuteTestSounds(true);
    }

    public void CloseAllMenus()
    {
        PauseMenu.SetActive(false);
        AudioSettingsMenu.SetActive(false);
        VideoSettingsMenu.SetActive(false);
        RestartConfirmMenu.SetActive(false);
        ExitConfirmMenu.SetActive(false);
        Settings.Instance.PauseSounds(false);
        Settings.Instance.MuteTestSounds(true);
    }

    public void GoToMainMenu()
    {
        ResumeGame();
        GameManager.Instance.LoadScene(GameScene.MAIN_MENU);
    }

    public void RestartFromCheckpoint()
    {
        CloseAllMenus();
        ResumeGame();
        // respawn from level manager or something?
    }

    public void RestartLevel()
    {
        GameManager.Instance.HandlePause();
        if (LevelManager.Instance == null)
        {
            Debug.LogWarning("Pause manager attempting to restart level but LevelManager not found!");
            return;
        }
        LevelManager.Instance.RestartLevel();
    }

    public void QuitLevel()
    {
        GameManager.Instance.HandlePause();
        DataPersistenceManager.Instance.SaveGame();
        GameManager.Instance.LoadScene(GameScene.MAIN_MENU);
    }
}
