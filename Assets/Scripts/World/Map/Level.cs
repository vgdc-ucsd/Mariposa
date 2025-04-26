using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField]
    public Sublevel[] Sublevels;

    

    public void LoadSublevel(int level)
    {
        Sublevels[level].Load();
    }

    public void UnloadSublevel(int level)
    {
        Sublevels[level].Unload();
    }
}
