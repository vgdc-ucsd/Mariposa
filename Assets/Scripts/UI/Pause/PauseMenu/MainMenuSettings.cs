using UnityEngine;

public class MainMenuSettings : Singleton<MainMenuSettings>
{
    public GameObject SettingsButtons;
    public GameObject VideoSettingsMenu;
    public GameObject AudioSettingsMenu;

    public void OpenVideoSettings()
    {
        SettingsButtons.SetActive(false);
        AudioSettingsMenu.SetActive(false);
        VideoSettingsMenu.SetActive(true);
    }

    public void OpenAudioSettings()
    {
        SettingsButtons.SetActive(false);
        AudioSettingsMenu.SetActive(true);
        VideoSettingsMenu.SetActive(false);
    }

    public void CloseAllMenus()
    {
        SettingsButtons.SetActive(true);
        AudioSettingsMenu.SetActive(false);
        VideoSettingsMenu.SetActive(false);
    }
}
