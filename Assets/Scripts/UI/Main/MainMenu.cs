using System.Collections;
using UnityEngine;

//To assign a method to the onclick for a button...
//Drag and drop the menuManager object into the button onclick on unity
//Drop down of methods in this script should show up and you can then choose

public class MainMenu : Singleton<MainMenu>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MusicManager.Instance.ChangeMusic(AudioEvents.Music.titlescreen_title);
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
        GameManager.Instance.LoadScene(GameManager.Instance.SavedScene);
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
        DataPersistenceManager.Instance.SaveGame();
        Application.Quit();
    }

}
