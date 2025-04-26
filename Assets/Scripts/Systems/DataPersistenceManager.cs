using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// Manages saving and loading data from disk and the distribution of that data
/// to components with the IDataPersistence interface.
/// Reading/Writing of data to disk is handled by the FileDataManager
/// </summary>
public class DataPersistenceManager : Singleton<DataPersistenceManager>
{
    [Header("File Storage Config")]
    public string fileName = "";

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataManager dataManager;
    

    private void Start()
    {
        // Initialize the file data manager with the default file path and the file name given by the serialize field
        this.dataManager = new FileDataManager(Application.persistentDataPath);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

    }

    /// <summary>
    /// Makes a new GameData object with default data values, as defined
    /// in the GameData constructor
    /// </summary>
    public void NewGame()
    {
        this.gameData = new GameData();
    }

    /// <summary>
    /// Loads the gameData object from disk using the FileDataManager.
    /// After loading, the data is passed to each component with the IDataPersistence interface
    /// and then calls the LoadGame() method on each of those components
    /// </summary>
    public void LoadGame(string fileName)
    {
        SetCurrentSave(fileName);

        gameData = dataManager.Load();
        if (this.gameData == null)
        {
            Debug.Log("No data was found, using default data values");
            NewGame();
        }
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    /// <summary>
    /// Passes a reference of gameData to each component with the IDataPersistence interface and then
    /// calls the SaveGame() method on each of those components
    /// </summary>
    public void SaveGame(string fileName)
    {
        SetCurrentSave(fileName);
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        dataManager.Save(gameData);
    }

    /// <summary>
    /// Delete the save data at the specified file location in the FileDataManager
    /// </summary>
    public void DeleteGame(string fileName)
    {
        dataManager.DeleteSave(fileName);
    }

    // Save the game when the application closes
    // This can be changed later!!!!!!
    private void OnApplicationQuit()
    {
        SaveGame("debug");
    }
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                                                                .OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    /// <summary>
    /// Helper function that updates the file that the data persistence manager and the file data manager will modify
    /// </summary>
    /// <param name="saveSlot">Name of the file you want to use</param>
    private void SetCurrentSave(string fileName)
    {
        this.fileName = fileName;
        dataManager.SetCurrentSaveSlot(fileName);
    }
}
