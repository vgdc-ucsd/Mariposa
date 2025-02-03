using UnityEngine;

public class PersistentManager : Singleton<PersistentManager>
{
    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
