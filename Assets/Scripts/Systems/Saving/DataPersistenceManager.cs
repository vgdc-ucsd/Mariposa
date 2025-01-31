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
    [SerializeField] private string fileName;


    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataManager dataManager;
    

    private void Start()
    {
        this.dataManager = new FileDataManager(Application.persistentDataPath, fileName);
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
    public void LoadGame()
    {
        this.gameData = dataManager.Load();
        if (this.gameData == null)
        {
            Debug.Log("No data was found, using default data values");
            NewGame();
        }
        // TODO - Make this method load the game data from disk using the file data handler
        // TODO - Make this method pass data to every Monobehavior component that implements
        //        the IDataPersistence interface and call the LoadGame() method on each of those components
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    /// <summary>
    /// Passes a reference of gameData to each component with the IDataPersistence interface and then
    /// calls the SaveGame() method on each of those components
    /// </summary>
    public void SaveGame()
    {
        // TODO - give data to other scripts to actually save data
        // TODO - make it save the json file to disk with a separate file data handler
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        dataManager.Save(gameData);
    }
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                                                                .OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
