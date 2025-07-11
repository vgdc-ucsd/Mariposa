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

    // Player inventory
    public Inventory mariposaInventory;
    public Inventory unnamedInventory;

    //Settings
    public AudioSetting audioSetting;
    // Video Settings
    public int width;
    public int height;
    public WindowType windowType;

    public int currentSublevelIndex;
    public string nextLevelScene;
    /// <summary>
    /// Constructor for the GameData object
    /// Creates a new object with the default values
    /// </summary>
    public GameData()
    {
        // Default Creations may Conflict with Start() calls
        // Probably don't even need these anyway or we can refactor the code for this to work
        mariposaInventory = new Inventory();
        unnamedInventory = new Inventory();
        this.audioSetting = new AudioSetting();
        this.width = 1920;
        this.height = 1080;
        this.windowType = WindowType.Windowed;

        this.TEST_keyStrokeCount = 0;
        this.currentSublevelIndex = 0;
        this.nextLevelScene       = "TutorialMockup";
    }
}