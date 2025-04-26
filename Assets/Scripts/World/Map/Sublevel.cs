using UnityEngine;

public class Sublevel : MonoBehaviour
{
    [SerializeField]
    private CharID _subLevelCharacter;
    public CharID SublevelCharacter { 
        get => _subLevelCharacter; 
    }

    public RespawnPoint StartingSpawn;
    public Collider2D CameraBounds;

    private void Awake()
    {
        if (StartingSpawn == null) Debug.LogError("Starting Spawn Points not assigned");
        if (CameraBounds == null) Debug.LogError("Starting Camera Bounds not assigned");
    }

    public void Load()
    {
        gameObject.SetActive(true);
        enabled = true;
    }

    public void Unload()
    {
        gameObject.SetActive(false);
        enabled = false;
    }
}
