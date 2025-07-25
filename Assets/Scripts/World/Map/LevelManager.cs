using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class LevelManager : Singleton<LevelManager>
{
    public Level CurrentLevel;

    void Start()
    {
        //DataPersistenceManager.Instance.SaveGame(DataPersistenceManager.Instance.fileName);
        CurrentLevel.LoadLevel();
        FadeController.Instance.FadeIn();
    }

    public void GoToNextSublevel()
    {
        CurrentLevel.GoToNextSublevel();
    }

    public void LoadNextLevel()
    {
        FadeController.Instance.FadeOutAndDo(() =>
        {
            GameManager.Instance.LoadScene(CurrentLevel.NextScene);
        });
    }

    public void RestartLevel()
    {
        GameManager.Instance.LoadScene(GameManager.Instance.CurrentScene);
    }

    public void RestartFromCheckpoint()
    {
        CurrentLevel.RestartFromCheckpoint();
    }
    

/* 
        public void SaveData(ref GameData data)
        {
            CurrentLevel.CurLevelIndex = SceneManager.GetActiveScene().buildIndex;
            data.currentSublevelIndex = SublevelIndex;
            data.nextLevelScene = NextLevelName;
            data.curLevel = CurrentLevel;
            data.ActiveEnemies = ActiveEnemies;
            data.Breakables = Breakables;

        }

        public void LoadData(GameData data)
        {
            SublevelIndex = data.currentSublevelIndex;
            NextLevelName = data.nextLevelScene;
            CurrentLevel = data.curLevel;
            ActiveEnemies = data.ActiveEnemies;
            Breakables = data.Breakables;

            SceneManager.LoadScene(CurrentLevel.CurLevelIndex);
            CurrentLevel.LoadSublevel(SublevelIndex);
            // TODO: Check if it works and if needed call a function to apply the changes
        }
        */
}
