using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class BigRobotLevel : MonoBehaviour
{
    public static BigRobotLevel Instance;

    [SerializeField] private ChaseRobot[] chaseRobots;
    [SerializeField] private BigRobotCutscene cutscene;
    [SerializeField] private RespawnPoint[] checkpoints;
    [SerializeField] private BigRobotLevelTrigger[] triggers;

    [SerializeField] private Vector2 climbRobotSpecialResetPosition;

    private WaitForSeconds waitForFade;

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
        CHASE_START,
        DROP
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

        waitForFade = new(FadeController.Instance._fadeDuration);
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

        currentSection = CurrentSection.START;

        RuntimeManager.StudioSystem.setParameterByName(MUSIC_PARAM, (int)MusicSection.CHASE_START);

        foreach (ChaseRobot robot in chaseRobots)
        {
            robot.StopAllCoroutines();
            robot.state = ChaseRobot.RobotWallState.IDLE;
            robot.gameObject.SetActive(false);
        }
        SendRobot(0);
    }

    private void OnDisable()
    {
        Player.OnDeath -= OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        StartCoroutine(DisableAllRobots());
        if (Player.ActivePlayer.CurrentRespawnPoint == checkpoints[0])
        {
            currentSection = CurrentSection.START;
            StartCoroutine(ResetRobot(0));
        }
        else if (Player.ActivePlayer.CurrentRespawnPoint == checkpoints[1])
        {
            currentSection = CurrentSection.CLIMB;
            StartCoroutine(ResetRobot(1));
        }
        else if (Player.ActivePlayer.CurrentRespawnPoint == checkpoints[2])
        {
            currentSection = CurrentSection.CLIMB;
            StartCoroutine(ResetRobot(1));
        }
        else if (Player.ActivePlayer.CurrentRespawnPoint == checkpoints[3])
        {
            currentSection = CurrentSection.SPIKE_JUMPS;
            StartCoroutine(ResetRobot(5));
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
                SendRobot(5);
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
        LevelManager.Instance.LoadNextLevel();
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

    private IEnumerator ResetRobot(int robotIndex)
    {
        if (robotIndex < 0 || robotIndex >= chaseRobots.Length)
        {
            Debug.LogError($"{robotIndex} is not a valid index for chaseRobots");
            yield break;
        }

        yield return waitForFade;

        chaseRobots[robotIndex].gameObject.SetActive(true);
        chaseRobots[robotIndex].col.enabled = true;
        chaseRobots[robotIndex].ResetRobot();
    }

    private IEnumerator DisableAllRobots()
    {
        foreach (ChaseRobot robot in chaseRobots)
        {
            robot.StopAllCoroutines();
            robot.state = ChaseRobot.RobotWallState.IDLE;
            robot.col.enabled = false;
        }

        yield return waitForFade;

        foreach (ChaseRobot robot in chaseRobots)
        {
            robot.col.enabled = true;
            robot.gameObject.SetActive(false);
        }
    }
}
