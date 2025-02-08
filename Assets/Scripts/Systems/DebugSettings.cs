using UnityEngine;

[System.Serializable]
public class DebugSettings
{
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private bool playerDebug = false;
    [SerializeField] private bool respawnDebug = false;

    public bool GetPlayerDebug()
    {
        return enableDebug && playerDebug;
    }

    public bool GetRespawnDebug()
    {
        return enableDebug && respawnDebug;
    }
}
