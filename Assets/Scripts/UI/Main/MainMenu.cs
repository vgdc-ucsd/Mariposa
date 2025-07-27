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
        MusicManager.Instance.ChangeMusic(AudioEvents.Music.titlescreen_title_theme);
    }

    public void NewGameBtn()
    {
        DataPersistenceManager.Instance.NewGame();
        GameManager.Instance.LoadScene(GameScene.TUTORIAL);
    }

    public void LoadGameBtn()
    {
        DataPersistenceManager.Instance.LoadGame(DataPersistenceManager.Instance.fileName);
    }

    public void SettingsBtn()
    {
        // Opens via onClick event in inspector
    }

    public void ExitBtn()
    {
        Application.Quit();
    }

}
