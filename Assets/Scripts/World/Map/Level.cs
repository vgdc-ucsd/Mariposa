using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] public Sublevel[] Sublevels;
    public GameScene NextScene { get; }
    public int SublevelIndex { get; private set; }

    public void LoadLevel()
    {
        // TODO: Load inventory
        foreach (Sublevel sl in Sublevels) sl.Unload();
        SublevelIndex = 0;
        LoadSublevel(SublevelIndex);
    }

    public void LoadSublevel(int index)
    {
        Sublevels[index].Load();
    }

    public void UnloadSublevel(int index)
    {
        Sublevels[index].Unload();
    }

    public void GoToNextSublevel()
    {
        UnloadSublevel(SublevelIndex);
        SublevelIndex++;
        SublevelIndex %= Sublevels.Length;
        LoadSublevel(SublevelIndex);
    }

    public void RestartFromCheckpoint()
    {
        Sublevels[SublevelIndex].RestartFromCheckpoint();
    }
}
