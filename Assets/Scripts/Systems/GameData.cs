using UnityEngine;

/// <summary>
/// JSON serializable class that is used to store the game data. 
/// </summary>
[System.Serializable]
public class GameData
{
    // Player Data
    public string UnnamedName;
    public int friendshipScore;
    public GameScene SavedScene;

    // Video Settings
    public int width;
    public int height;
    public WindowType windowType;
    public int resolutionIndex;

    // Audio Settings
    public float sfxValue;
    public float musicValue;
    public float masterValue;
    public float ambienceValue;
    public float dialogueValue;

    public GameData()
    {
        ResetPlaythrough();

        width = Screen.width;
        height = 1080;
        windowType = WindowType.WindowedFullscreen;

    }

    public void ResetPlaythrough()
    {
        UnnamedName = "Kairo";
        friendshipScore = 0;
        SavedScene = GameScene.TUTORIAL;
    }
}