using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// JSON serializable class that is used to store the game data. 
/// </summary>
[System.Serializable]
public class GameData
{
    // REMOVE THIS VARIABLE LATER! This is just to test the saving :)
    public int TEST_keyStrokeCount;

    // Player Data
    // TODO: Add reference to Unnamed's name
    public int friendshipScore;

    // Settings
    public AudioSetting audioSetting;
    // Video Settings
    public int width;
    public int height;
    public WindowType windowType;

    // Level Manager
    public Level curLevel;
    public int currentSublevelIndex;
    public string nextLevelScene;
    public List<Enemy> ActiveEnemies;
    public List<BreakablePlatform> Breakables;

    /// <summary>
    /// Constructor for the GameData object
    /// Creates a new object with the default values
    /// </summary>
    public GameData()
    {
        // Default Creations may Conflict with Start() calls
        // Probably don't even need these anyway or we can refactor the code for this to work
        this.friendshipScore = 0;
        this.audioSetting = new AudioSetting();
        this.width = 1920;
        this.height = 1080;
        this.windowType = WindowType.Windowed;

        this.TEST_keyStrokeCount = 0;

        this.curLevel = new Level(); //Default Nothing
        this.ActiveEnemies = new List<Enemy>();
        this.Breakables = new List<BreakablePlatform>();
        this.currentSublevelIndex = 0;
        this.nextLevelScene       = "TutorialMockup";
    }
}