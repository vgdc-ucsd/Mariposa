using UnityEngine;

public class FileDataManager : Singleton<FileDataManager>
{
    private string dataDirPath = "";
    private string dataFileName= "";
    /// <summary>
    /// Constructor for the FileDataHandler class
    /// Sets the directory and file name of the save file
    /// </summary>
    /// <param name="dataDirPath">Sets the directory where data is saved in</param>
    /// <param name="dataFileName">Sets the name of the save file</param>
    public void FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }
    public GameData Load()
    {
        // TODO - Make this method load the GameData object from disk
        //        and return the data loaded
    }
    public void Save()
    {
        // TODO - Make this method serialize and save the current GameData object to disk
        // TODO - Make this method make a new directory if the directory given by dataDirPath doesn't exist yet
    }
}
