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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NewGameBtn()
    {
        SceneManager.LoadScene(1);
        Debug.Log("New Game button clicked");
    }

    public void LoadGameBtn()
    {
        Debug.Log("Load button clicked");

    }

    public void SettingsBtn()
    {
        Debug.Log("Settings button clicked");

    }

    public void ExitBtn()
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }

}
