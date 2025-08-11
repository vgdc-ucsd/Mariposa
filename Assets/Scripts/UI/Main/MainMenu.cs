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
        GameManager.Instance.LoadScene(GameScene.TUTORIAL);
        DataPersistenceManager.Instance.SaveGame();
    }

    public void LoadGameBtn()
    {
        DataPersistenceManager.Instance.LoadGame();
        GameManager.Instance.LoadScene(GameManager.Instance.CurrentScene);
    }

    public void SettingsBtn()
    {
        // Opens via onClick event in inspector
    }

    public void ExitBtn()
    {
        DataPersistenceManager.Instance.SaveGame();
        Application.Quit();
    }

}
