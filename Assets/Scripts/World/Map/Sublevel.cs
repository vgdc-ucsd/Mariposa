using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AudioEvents;

public class Sublevel : MonoBehaviour
{
    [SerializeField] private CharID _subLevelCharacter;
    public RespawnPoint StartingSpawn;
    public Collider2D CameraBounds;

    [field: SerializeField, Header("Audio Properties")] public Music SublevelMusic { get; private set; } = Music.NONE;
    [field: SerializeField] public Ambience SublevelAmbience { get; private set; } = Ambience.NONE;
    [field: SerializeField] public float MusicTransitionDuration { get; private set; } = 1.5f;
    [field: SerializeField] public bool PlayOnLoad { get; private set; } = true;
    private List<Enemy> activeEnemies;
    private List<BreakablePlatform> breakables;

    private void Awake()
    {
        if (StartingSpawn == null) Debug.LogError("Starting Spawn Points not assigned");
        if (CameraBounds == null) Debug.LogError("Starting Camera Bounds not assigned");
    }

    public void Load()
    {
        CameraController.ActiveCamera.SetBounds(CameraBounds);

        activeEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
        breakables = FindObjectsByType<BreakablePlatform>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        ResetEnemies();
        ResetBreakables();

        // Tell audio managers to change audio if PlayOnLoad is on in the current sublevel
        if (PlayOnLoad)
        {
            MusicManager.Instance.ChangeMusic(SublevelMusic, MusicTransitionDuration);
            AmbienceManager.Instance.ChangeAmbience(SublevelAmbience);
        }

        gameObject.SetActive(true);
        CameraManager.Instance.GetCamerasInScene();
        CameraManager.Instance.ResetCamera();
        PlayerController.Instance.LoadIntoSublevel(_subLevelCharacter, StartingSpawn.GetRespawnPosition());
    }

    public void Unload()
    {
        gameObject.SetActive(false);
    }

    public void RestartFromCheckpoint()
    {
        ResetEnemies();
        ResetBreakables();
    }

    public void ResetEnemies()
    {
        foreach (Enemy enemy in activeEnemies)
        {
            enemy.Init();
        }
    }

    public void ResetBreakables()
    {
        foreach (BreakablePlatform platform in breakables)
        {
            platform.Reset();
        }
    }
}
