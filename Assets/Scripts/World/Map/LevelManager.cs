using UnityEngine;
using System.Collections;
using NUnit.Framework;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public Level CurrentLevel;
    public int SublevelIndex { get; private set; }

    private void Awake()
    {
        Instance = this;
        foreach (Sublevel sublevel in CurrentLevel.Sublevels)
        {
            sublevel.Unload();
        }
        SublevelIndex = 0;
        CurrentLevel.LoadSublevel(SublevelIndex);
        
    }

    private void Start()
    {
        InitSublevel();
    }

    private void Update()
    {
        // Swap worlds when the "F" key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            GoToNextSublevel();
        }
    }

    private Sublevel GetCurrentSublevel() => CurrentLevel.Sublevels[SublevelIndex];

    public void GoToNextSublevel()
    {
        CurrentLevel.UnloadSublevel(SublevelIndex);
        SublevelIndex++;
        SublevelIndex %= CurrentLevel.Sublevels.Length;
        CurrentLevel.LoadSublevel(SublevelIndex);
        InitSublevel();
        
    }

    private void InitSublevel()
    {
        PlayerController.Instance.SwitchTo(GetCurrentSublevel().SublevelCharacter);
        CameraController.ActiveCamera.SetBounds(GetCurrentSublevel().CameraBounds);
        Player.ActivePlayer.transform.position = GetCurrentSublevel().StartingSpawn.GetRespawnPosition();
        if (Player.ActivePlayer.Ability is BeeControlAbility bc)
        {
            bc.BeeRef.transform.position = Player.ActivePlayer.transform.position + new Vector3(0, 2, 0);
        }
        Debug.Assert(GetCurrentSublevel().SublevelCharacter == Player.ActivePlayer.Character.Id);
    }
}
