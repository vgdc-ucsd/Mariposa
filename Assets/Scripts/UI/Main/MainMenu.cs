using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//To assign a method to the onclick for a button...
//Drag and drop the menuManager object into the button onclick on unity
//Drop down of methods in this script should show up and you can then choose

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MusicManager.Instance.ChangeMusic(AudioEvents.Music.titlescreen_title_theme);
        PauseMenuScript.Instance.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        PauseMenuScript.Instance.gameObject.SetActive(true);
    }

    public void NewGameBtn()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadGameBtn()
    {
        DataPersistenceManager.Instance.LoadGame(DataPersistenceManager.Instance.fileName);
    }

    public void SettingsBtn()
    {
    }

    public void ExitBtn()
    {
        Application.Quit();
    }

}
