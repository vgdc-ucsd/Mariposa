using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public Level CurrentLevel;
    public string NextLevelName;
    public int SublevelIndex { get; private set; }

    public List<Enemy> ActiveEnemies;


    private void Awake()
    {
        Instance = this;
        foreach (Sublevel sublevel in CurrentLevel.Sublevels)
        {
            sublevel.Unload();
        }
        SublevelIndex = GameManager.Instance.TargetSublevel;
        CurrentLevel.LoadSublevel(SublevelIndex);

        // does this even work?
        if (SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCount - 1) return;
        NextLevelName = SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1).name;

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

        if (Input.GetKeyDown(KeyCode.M))
        {
            LoadNextLevel();
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

    public void GoToPreviousLevel()
    {
        CurrentLevel.UnloadSublevel(SublevelIndex);
        SublevelIndex--;
        if (SublevelIndex <= 0)
        {
            Debug.LogWarning("no previous level; looping");
            SublevelIndex += CurrentLevel.Sublevels.Length;
        }
        CurrentLevel.LoadSublevel(SublevelIndex);
        InitSublevel();
    }

    private void InitSublevel()
    {
        // teleport previous player (and bee, if applicable) off screen
        Player.ActivePlayer.transform.position = new Vector3(-1000, -1000, 0);
        if (Player.ActivePlayer.Ability is BeeControlAbility b)
        {
            b.BeeRef.transform.position = new Vector3(-1000, -1000, 0);
        }
        PlayerController.Instance.SwitchTo(GetCurrentSublevel().SublevelCharacter);
        CameraController.ActiveCamera.SetBounds(GetCurrentSublevel().CameraBounds);
        Player.ActivePlayer.transform.position = GetCurrentSublevel().StartingSpawn.GetRespawnPosition();
        if (Player.ActivePlayer.Ability is BeeControlAbility bc)
        {
            bc.BeeRef.transform.position = Player.ActivePlayer.transform.position + new Vector3(0, 2, 0);
        }
        Debug.Assert(GetCurrentSublevel().SublevelCharacter == Player.ActivePlayer.Data.characterID);

        ActiveEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
        ResetEnemies();

        // Tell AudioManager to change audio if PlayOnLoad is on in the sublevel
        if (GetCurrentSublevel().PlayOnLoad)
        {
            MusicManager.Instance.ChangeEvent();
            AmbienceManager.Instance.ChangeEvent();
        }
    }

    public void ResetEnemies()
    {
        foreach (Enemy enemy in ActiveEnemies)
        {
            enemy.Init();
        }
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(NextLevelName);
        // Scene nextLevel = SceneManager.GetSceneByName(NextLevelName);
    }
}
