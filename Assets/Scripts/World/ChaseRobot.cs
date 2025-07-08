using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Splines;

public class ChaseRobot : MonoBehaviour
{
    public enum RobotWallState
    {
        IDLE,
        MOVING,
        ENTERING,
        RETREATING
    }

    [Header("Params")]
    public float baseMovementSpeed = 4.0f;
    [SerializeField] private float movementStartDelay = 1.0f;
    [SerializeField] private float catchupDistanceThreshold = 5.0f;
    [SerializeField] private float maxSpeed = 40.0f;
    [SerializeField] private float catchupRate = 5.0f;
    [SerializeField] private float perpendicularCatchupRate = 5.0f;

    [Header("Entering")]
    [SerializeField] private Transform chasePoint;
    [SerializeField] private float timeToEnter;
    [SerializeField] private float enterStartOffset;

    [Header("Retreating")]
    [SerializeField] private Transform retreatPoint;
    [SerializeField] private float retreatDistance;
    [SerializeField] private float timeToRetreat;
    [SerializeField] private ChaseRobot nextRobot;

    [Header("State")]
    public Vector2 moveDir;
    private float currentSpeed;
    public RobotWallState state;


    private Vector2 startPos;

    void Start()
    {
        startPos = transform.position;

        ResetRobot();
    }

    private void OnEnable()
    {
        Player.OnDeath += ResetRobot;
    }

    private void OnDisable()
    {
        Player.OnDeath -= ResetRobot;
    }

    private IEnumerator StartMoving()
    {
        state = RobotWallState.IDLE;

        yield return new WaitForSeconds(movementStartDelay);

        state = RobotWallState.MOVING;
    }

    private IEnumerator Enter()
    {
        yield return new WaitForEndOfFrame();

        float enterTime = 0.0f;
        float enterMoveSpeed = enterStartOffset / timeToEnter;
        transform.position = startPos - moveDir * enterStartOffset + (Vector2)GetPerpCorrection();
        state = RobotWallState.ENTERING;

        while (enterTime < timeToEnter)
        {
            transform.position += enterMoveSpeed * Time.fixedDeltaTime * (Vector3)moveDir;
            enterTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(StartMoving());
    }

    private IEnumerator Retreat()
    {
        float retreatTime = 0.0f;
        float retreatMoveSpeed = retreatDistance / timeToRetreat;
        state = RobotWallState.RETREATING;

        while (retreatTime < timeToRetreat)
        {
            transform.position -= retreatMoveSpeed * Time.fixedDeltaTime * (Vector3)moveDir;
            retreatTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        state = RobotWallState.IDLE;
        if (nextRobot != null) nextRobot.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        float fdt = Time.fixedDeltaTime;

        if (moveDir == Vector2.zero) return;
        if (state == RobotWallState.MOVING)
        {
            moveDir = moveDir.normalized;

            float targetSpeed;
            Vector2 playerPos = Player.ActivePlayer.transform.position;
            float distError = Helper.Vec2Proj(playerPos - (Vector2)transform.position, moveDir).magnitude - catchupDistanceThreshold;
            if (distError > 0)
            {
                float catchUpMul = Mathf.Clamp(distError / catchupDistanceThreshold, 0f, 2f);
                targetSpeed = Mathf.Lerp(baseMovementSpeed, maxSpeed, catchUpMul);
            }
            else
            {
                targetSpeed = baseMovementSpeed;
            }

            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, catchupRate * fdt);
            Vector3 movement = currentSpeed * fdt * moveDir;
            transform.position += movement;

            transform.position += Vector3.Lerp(Vector3.zero, GetPerpCorrection(), fdt * perpendicularCatchupRate);

            if (retreatPoint != null && DidPassRetreatPoint())
                StartCoroutine(Retreat());
        }
    }

    private Vector3 GetPerpCorrection()
    {
        Vector2 targetPos = Camera.main.transform.position;
        Vector2 separationToTarget = (Vector2)transform.position - targetPos;
        Vector3 correction = separationToTarget - Helper.Vec2Proj(separationToTarget, moveDir);
        return -correction;
    }

    public void ResetRobot()
    {
        transform.position = startPos;
        currentSpeed = baseMovementSpeed;

        StartCoroutine(Enter());
    }

    private bool DidPassRetreatPoint() => Vector2.Dot(retreatPoint.position - transform.position, moveDir) <= 0;
}
