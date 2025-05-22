using UnityEngine;
using static AudioEvents;

public class Sublevel : MonoBehaviour
{
    [SerializeField]
    private CharID _subLevelCharacter;
    public CharID SublevelCharacter { get => _subLevelCharacter; }

    public RespawnPoint StartingSpawn;
    public Collider2D CameraBounds;

    [field: SerializeField, Header("Audio Properties")] public Music SublevelMusic { get; private set; } = Music.NONE;
    [field: SerializeField] public Ambience SublevelAmbience { get; private set; } = Ambience.NONE;
    [field: SerializeField] public bool PlayOnLoad { get; private set; } = true;

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
