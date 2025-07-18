using FMODUnity;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BigRobotLevel : MonoBehaviour
{
    public static BigRobotLevel Instance;

    [SerializeField] private ChaseRobot[] chaseRobots;
    [SerializeField] private BigRobotCutscene cutscene;
    [SerializeField] private RespawnPoint[] checkpoints;
    [SerializeField] private BigRobotLevelTrigger[] triggers;

    public enum CurrentSection
    {
        START, 
        CLIMB,
        SPIKE_GRAPPLE,
        SPIKE_JUMPS,
        FINAL_CHASE
    };
    public CurrentSection currentSection;

    public enum MusicSection
    {
        CUTSCENE_START,
        CUTSCENE_END,
        PLAYER_DEATH
    };
    public const string MUSIC_PARAM = "s3_chase";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitForEndOfFrame();

        Player.ActivePlayer.gameObject.SetActive(false);
        cutscene.gameObject.SetActive(true);
        cutscene.PlayCutscene(onCutsceneEnd: StartLevel);
        RuntimeManager.StudioSystem.setParameterByName(MUSIC_PARAM, (int)MusicSection.CUTSCENE_START);
    }

    private void StartLevel()
    {
        Player.ActivePlayer.gameObject.SetActive(true);
        Player.ActivePlayer.CurrentRespawnPoint = checkpoints[0];
        Player.OnDeath += OnPlayerDeath;
        DisableAllRobots();
        cutscene.gameObject.SetActive(false);
        currentSection = CurrentSection.START;
        SendRobot(0);
    }

    private void OnDisable()
    {
        Player.OnDeath -= OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        DisableAllRobots();
        RuntimeManager.StudioSystem.setParameterByName(MUSIC_PARAM, (int)MusicSection.PLAYER_DEATH);
        MusicManager.Instance.Stop();
        MusicManager.Instance.Play();
        if (Player.ActivePlayer.CurrentRespawnPoint == checkpoints[0])
        {
            currentSection = CurrentSection.START;
            ResetRobot(0);
        }
        else if (Player.ActivePlayer.CurrentRespawnPoint == checkpoints[1])
        {
            currentSection = CurrentSection.CLIMB;
            ResetRobot(1);
        }
        else
        {
            Debug.Log("Invalid checkpoint");
        }
    }

    public void TriggerNextSection(CurrentSection section)
    {
        currentSection = section;
        switch (section)
        {
            case CurrentSection.START:
                break;
            case CurrentSection.CLIMB:
                triggers[(int)CurrentSection.CLIMB].gameObject.SetActive(false);
                SendRobot(1);
                break;
            case CurrentSection.SPIKE_GRAPPLE:
                SendRobot(2);
                break;
            case CurrentSection.SPIKE_JUMPS:
                break;
            case CurrentSection.FINAL_CHASE:
                SendRobot(3);
                SendRobot(4);
                break;
            default:
                Debug.LogError($"{section} is not a valid section");
                break;
        }
    }

    public void CompleteLevel()
    {
        Debug.Log("Level Complete!");
    }

    private void SendRobot(int robotIndex)
    {
        if (robotIndex < 0 || robotIndex >= chaseRobots.Length)
        {
            Debug.LogError($"{robotIndex} is not a valid index for chaseRobots");
            return;
        }

        chaseRobots[robotIndex].gameObject.SetActive(true);
        StartCoroutine(chaseRobots[robotIndex].Enter());
    }

    private void ResetRobot(int robotIndex)
    {
        if (robotIndex < 0 || robotIndex >= chaseRobots.Length)
        {
            Debug.LogError($"{robotIndex} is not a valid index for chaseRobots");
            return;
        }

        chaseRobots[robotIndex].gameObject.SetActive(true);
        StartCoroutine(chaseRobots[robotIndex].ResetRobot());
    }

    private void DisableAllRobots()
    {
        foreach (ChaseRobot robot in chaseRobots)
        {
            robot.StopAllCoroutines();
            robot.state = ChaseRobot.RobotWallState.IDLE;
            robot.gameObject.SetActive(false);
        }
    }
}
