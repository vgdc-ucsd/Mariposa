using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
/// <summary>
/// Manages saving and loading data from disk and the distribution of that data
/// to components with the IDataPersistence interface.
/// Reading/Writing of data to disk is handled by the FileDataManager
/// </summary>
public class DataPersistenceManager : Singleton<DataPersistenceManager>
{
    // [Header("File Storage Config")]
    // Do not make this public, file I/O exception
    private string fileName = "save.json";
    public GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;
    private string fullPath;

    private void Start()
    {
        fullPath = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}{fileName}";
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        
        LoadGame();
    }

    /// <summary>
    /// Makes a new GameData object with default data values, as defined
    /// in the GameData constructor
    /// </summary>
    public void NewGame()
    {
        gameData = new GameData();
    }

    /// <summary>
    /// Loads the gameData object from disk using the FileDataManager.
    /// After loading, the data is passed to each component with the IDataPersistence interface
    /// and then calls the LoadGame() method on each of those components
    /// </summary>
    public void LoadGame()
    {
        if (File.Exists(fullPath))
        {
            string saveData = File.ReadAllText(fullPath);
            gameData = JsonUtility.FromJson<GameData>(saveData);
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                dataPersistenceObj.LoadData(gameData);
            }
        }
        else
        {
            Debug.Log("No data was found, creating new save file");
            NewGame();
        }
    }

    /// <summary>
    /// Passes a reference of gameData to each component with the IDataPersistence interface and then
    /// calls the SaveGame() method on each of those components
    /// </summary>
    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        File.WriteAllText(fullPath, JsonUtility.ToJson(gameData, true));
    }

    /// <summary>
    /// Delete the save data at the specified file location in the FileDataManager
    /// </summary>
    public void DeleteGame(string fileName)
    {
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            gameData = new GameData();
        }
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                                                                .OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
