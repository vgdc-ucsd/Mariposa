using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Handles the reading and writing to disk for the DataPersistanceManager
/// </summary>
public class FileDataManager
{
    private string dataDirPath = "";
    private string dataFileName= "";
    /// <summary>
    /// Constructor for the FileDataHandler class
    /// Sets the directory and file name of the save file
    /// </summary>
    /// <param name="dataDirPath">Sets the directory where data is saved in</param>
    /// <param name="dataFileName">Sets the name of the save file</param>
    public FileDataManager(string dataDirPath)
    {
        this.dataDirPath = dataDirPath;
    }

    /// <summary>
    /// Loads the json data in the path specified into a GameData object 
    /// </summary>
    /// <returns>Returns a GameData object</returns>
    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        GameData loadedData = null;
        if (File.Exists(fullPath)) 
        {
            try
            {
                // Deserialize the JSON back into a C# object
                loadedData = JsonUtility.FromJson<GameData>(File.ReadAllText(fullPath));
                Debug.Log("Loaded data successfuly from: " + fullPath);
            }
            catch (Exception e)
            {
                Debug.LogError("Error when trying to read data from file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    /// <summary>
    /// Serializes the inputted GameData object to JSON and then saves it to disk
    /// </summary>
    /// <param name="data">GameData object to be serialized into JSON</param>
    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            string dataToStore = JsonUtility.ToJson(data, true);
            Debug.Log("Saved data successfuly to: " + fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    public void DeleteSave(string saveName)
    {
        string fullPath = Path.Combine(dataDirPath, saveName);
        if (File.Exists(fullPath))
        {
            try
            {
                File.Delete(fullPath);
            }
            catch (Exception e)
            {
                Debug.LogError("Error when trying to delete save data at: " + fullPath + "\n" + e);
            }
        }
        else
        {
            Debug.Log("Tried to delete save data that doesn't exist at " + fullPath);
        }
    }

    public void SetCurrentSaveSlot(string saveSlot)
    {
        this.dataFileName = saveSlot;
    }

    /// <summary>
    /// Checks if the currently selected save is a valid save and isn't corrupted or has incorrect data types
    /// </summary>
    /// <returns>Boolean</returns>
    private bool ValidateSave()
    {
        // TODO: Make this check the data types on the save
        return true;
    }
}
