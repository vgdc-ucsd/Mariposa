using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField]
    private Sublevel[] sublevels;

    public int CurrentLevel { get; private set; }

    public void LoadSublevel(int level)
    {
        sublevels[level].Load();
    }

    public void UnloadSublevel(int level)
    {
        sublevels[level].Unload();
    }
}
