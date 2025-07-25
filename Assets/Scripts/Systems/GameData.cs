/// <summary>
/// JSON serializable class that is used to store the game data. 
/// </summary>
[System.Serializable]
public class GameData
{
    // Player inventory
    public Inventory mariposaInventory;
    public Inventory unnamedInventory;

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
        mariposaInventory = new Inventory();
        unnamedInventory = new Inventory();
        this.width = 1920;
        this.height = 1080;
        this.windowType = WindowType.Windowed;
    }
}