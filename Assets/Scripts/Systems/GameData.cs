/// <summary>
/// JSON serializable class that is used to store the game data. 
/// </summary>
[System.Serializable]
public class GameData
{
    // Player Data
    // TODO: Add reference to Unnamed's name
    public int friendshipScore;

    // Settings
    // TODO: audio settings

    // Video Settings
    public int width;
    public int height;
    public WindowType windowType;

    // Level Manager
    // TODO: level settings

    /// <summary>
    /// Constructor for the GameData object
    /// Creates a new object with the default values
    /// </summary>
    public GameData()
    {
        // Default Creations may Conflict with Start() calls
        // Probably don't even need these anyway or we can refactor the code for this to work
        this.friendshipScore = 0;
        this.width = 1920;
        this.height = 1080;
        this.windowType = WindowType.Windowed;
    }
}