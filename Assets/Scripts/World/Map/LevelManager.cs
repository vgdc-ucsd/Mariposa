    using UnityEngine;
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine.SceneManagement;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.VisualScripting;

    public class LevelManager : MonoBehaviour, IDataPersistence
    {
        public static LevelManager Instance;

        public Level CurrentLevel;
        public string NextLevelName;
        public int SublevelIndex { get; private set; }

        public List<Enemy> ActiveEnemies;
        public List<BreakablePlatform> Breakables;


        private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var sl in CurrentLevel.Sublevels) sl.Unload();
        SublevelIndex = GameManager.Instance.TargetSublevel;
        CurrentLevel.LoadSublevel(SublevelIndex);

        if (string.IsNullOrEmpty(NextLevelName))
        {
            var nextBuild = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextBuild < SceneManager.sceneCountInBuildSettings)
                NextLevelName = System.IO.Path.GetFileNameWithoutExtension(
                    SceneUtility.GetScenePathByBuildIndex(nextBuild)
                );
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FadeController.Instance.FadeIn();

            if (scene.name == NextLevelName)
            {
                StartCoroutine(InitSublevelDelayed());
            }
        }
        
        private IEnumerator InitSublevelDelayed()
        {
            while (
                FindObjectOfType<Level>() == null ||
                Player.ActivePlayer == null ||
                CameraController.ActiveCamera == null
            )
                yield return null;

            yield return null;  

            CurrentLevel = FindObjectOfType<Level>();
            SublevelIndex = 0;

            foreach (var sl in CurrentLevel.Sublevels) sl.Unload();
            CurrentLevel.LoadSublevel(SublevelIndex);
            InitSublevel();
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

    public void InitSublevel()
    {
        // teleport previous player (and bee, if applicable) off screen
        Player.ActivePlayer.transform.position = new Vector3(-1000, -1000, 0);
        if (Player.ActivePlayer.Ability is BeeControlAbility b)
        {
            b.BeeRef.transform.position = new Vector3(-1000, -1000, 0);
            b.TurnOffBeeFlap();
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
        // update grapple targets every time player switches to an unnamed sublevel
        GrappleAbility grappleAbility = Player.ActivePlayer.GetComponentInChildren<GrappleAbility>();
        if (grappleAbility != null && Player.ActivePlayer.Data.characterID == CharID.Unnamed)
        {
            grappleAbility.UpdateGrappleTargets();
        }

        ActiveEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
        Breakables = FindObjectsByType<BreakablePlatform>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        ResetEnemies();
        ResetBreakables();

        // Tell audio managers to change audio if PlayOnLoad is on in the current sublevel
        if (GetCurrentSublevel().PlayOnLoad)
        {
            MusicManager.Instance.ChangeMusic(MusicManager.Instance.GetMusicToCurrentSublevel(), GetCurrentSublevel().MusicTransitionDuration);
            AmbienceManager.Instance.ChangeAmbience(AmbienceManager.Instance.GetAmbienceToCurrentSublevel());
        }
    }

        public void ResetEnemies()
        {
            foreach (Enemy enemy in ActiveEnemies)
            {
                enemy.Init();
            }
        }
        
        public void ResetBreakables()
        {
            foreach (BreakablePlatform platform in Breakables)
            {
                platform.Reset();
            }
        }

        public void LoadNextLevel()
    {
        SceneManager.LoadScene(NextLevelName);
        // Scene nextLevel = SceneManager.GetSceneByName(NextLevelName);
    }
        
        public void SaveData(ref GameData data)
        {
            data.currentSublevelIndex = SublevelIndex;
            data.nextLevelScene       = NextLevelName;
        }
        
        public void LoadData(GameData data)
        {
            SublevelIndex  = data.currentSublevelIndex;
            NextLevelName  = data.nextLevelScene;
        }
        
        private void CompleteLevel()
        {
            SublevelIndex = 0;
            DataPersistenceManager.Instance.SaveGame(DataPersistenceManager.Instance.fileName);
            FadeController.Instance.FadeOutAndDo(() =>
            {
                SceneManager.LoadScene(NextLevelName);
            });
        }
    }
