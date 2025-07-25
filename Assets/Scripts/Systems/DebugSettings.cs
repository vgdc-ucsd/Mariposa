using UnityEngine;

[System.Serializable]
public class DebugSettings
{
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private bool playerDebug = false;
    [SerializeField] private bool respawnDebug = false;
    [SerializeField] private bool audioDebug = false;
    [SerializeField] private bool puzzleDebug = false;

    public bool GetDebug()
    {
        return enableDebug;
    }
    
    public bool GetPlayerDebug()
    {
        return enableDebug && playerDebug;
    }

    public bool GetRespawnDebug()
    {
        return enableDebug && respawnDebug;
    }

    public bool GetAudioDebug()
    {
        return enableDebug && audioDebug;
    }

    public bool GetPuzzleDebug()
    {
        return enableDebug && puzzleDebug;
    }
}
