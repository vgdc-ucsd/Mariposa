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

    private void GoToNextSublevel()
    {
        CurrentLevel.UnloadSublevel(SublevelIndex);
        SublevelIndex++;
        CurrentLevel.LoadSublevel(SublevelIndex);
        PlayerController.Instance.SwitchCharacters();
        Debug.Assert(GetCurrentSublevel().SublevelCharacter == Player.ActivePlayer.Character.Id);
        
    }
}
