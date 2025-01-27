using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Interface for the SaveData and LoadData methods
/// Use for any component that needs to have data persist after program close
/// LoadData will provide a GameData json object
/// SaveData gives a reference to a GameData json object that can be modified
/// </summary>
public interface IDataPersistance
{
    void LoadData(GameData data);
    void SaveData(ref GameData data);
}