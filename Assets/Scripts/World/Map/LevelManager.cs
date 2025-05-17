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


        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
            if (SublevelIndex + 1 >= CurrentLevel.Sublevels.Length)
            {
                CompleteLevel();
                return;
            }

            // otherwise just advance
            CurrentLevel.UnloadSublevel(SublevelIndex);
            SublevelIndex++;
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
