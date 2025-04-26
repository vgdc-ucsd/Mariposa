using UnityEngine;

public class LoadDeleteButtonManager : MonoBehaviour
{
    public DataPersistenceManager dataManager;
    public void DeleteSave(string fileName)
    {
        dataManager.DeleteGame(fileName);
    }
    public void LoadSave(string fileName)
    {
        dataManager.LoadGame(fileName);
    }
}
