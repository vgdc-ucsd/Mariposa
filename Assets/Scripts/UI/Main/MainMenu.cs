using UnityEngine;

//To assign a method to the onclick for a button...
//Drag and drop the menuManager object into the button onclick on unity
//Drop down of methods in this script should show up and you can then choose

public class MainMenu : Singleton<MainMenu>
{
    [SerializeField] private UnityEngine.UI.Button loadButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MusicManager.Instance.ChangeMusic(AudioEvents.Music.titlescreen_title_theme);

        // If no saved game, disable load button
        if (!DataPersistenceManager.Instance.HasSavedGame())
        {
            if (loadButton != null)
            {
                loadButton.interactable = false;
            }
        }
    }

    public void NewGameBtn()
    {
        DataPersistenceManager.Instance.gameData.ResetPlaythrough();
        GameManager.Instance.LoadScene(GameScene.TUTORIAL);
        FriendshipManager.Instance.SetScore(DataPersistenceManager.Instance.gameData.friendshipScore);
        DataPersistenceManager.Instance.SaveGame();
    }

    public void LoadGameBtn()
    {
        Debug.Log("Loading saved game, level: " + DataPersistenceManager.Instance.gameData.SavedScene);
        GameManager.Instance.LoadScene(DataPersistenceManager.Instance.gameData.SavedScene);
    }

    public void OpenAudioSettingsBtn()
    {
        PauseMenuScript.Instance.OpenAudioSettings();
    }

    public void OpenVideoSettingsBtn()
    {
        PauseMenuScript.Instance.OpenVideoSettings();
    }

    public void ExitBtn()
    {
        // DataPersistenceManager.Instance.SaveGame();
        Application.Quit();
    }

}
