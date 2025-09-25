using UnityEngine;

[System.Serializable]
public class DebugSettings
{
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private bool playerDebug = false;
    [SerializeField] private bool respawnDebug = false;
    [SerializeField] private bool audioDebug = false;
    [SerializeField] private bool puzzleDebug = false;

    public bool DebugEnabled => enableDebug;
    public bool PlayerDebugEnabled => enableDebug && playerDebug;
    public bool RespawnDebugEnabled => enableDebug && respawnDebug;
    public bool AudioDebugEnabled => enableDebug && audioDebug;
    public bool PuzzleDebugEnabled => enableDebug && puzzleDebug;
}
