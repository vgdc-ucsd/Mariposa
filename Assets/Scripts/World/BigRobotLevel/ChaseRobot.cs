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

    public Collider2D col;

    [Header("Params")]
    public float baseMovementSpeed = 4.0f;
    public float movementStartDelay = 1.0f;
    public float catchupDistanceThreshold = 5.0f;
    public float maxSpeed = 40.0f;
    public float catchupRate = 5.0f;
    public float perpendicularCatchupRate = 5.0f;

    [Header("Entering")]
    public float enterDelay;
    public float timeToEnter;
    public float enterStartOffset;

    [Header("Retreating")]
    public Transform retreatPoint;
    public float retreatDistance;
    public float timeToRetreat;

    [Header("State")]
    public Vector2 moveDir;
    private float currentSpeed;
    private bool hasPreviouslyEntered;
    public RobotWallState state;


    public Vector2 startPos;

    private void Awake()
    {
        startPos = transform.position;
        hasPreviouslyEntered = false;
        col = GetComponent<Collider2D>();
    }

    public IEnumerator StartMoving()
    {
        state = RobotWallState.IDLE;

        yield return new WaitForSeconds(movementStartDelay);

        state = RobotWallState.MOVING;
    }

    public IEnumerator Enter()
    {
        float enterTime = 0.0f;
        float enterMoveSpeed = enterStartOffset / timeToEnter;
        transform.position = startPos - moveDir * enterStartOffset;
        state = RobotWallState.ENTERING;
        hasPreviouslyEntered = true;

        if (!hasPreviouslyEntered) yield return new WaitForSeconds(enterDelay);

        while (enterTime < timeToEnter)
        {
            transform.position += enterMoveSpeed * Time.fixedDeltaTime * (Vector3)moveDir;
            enterTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(StartMoving());
    }

    public IEnumerator Retreat()
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
                float catchUpMul = Mathf.Clamp01(distError / catchupDistanceThreshold);
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
        else if (state == RobotWallState.ENTERING)
        {
            transform.position += Vector3.Lerp(Vector3.zero, GetPerpCorrection(), fdt * perpendicularCatchupRate);
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
        state = RobotWallState.IDLE;
        transform.position = startPos;
        currentSpeed = baseMovementSpeed;

        StartCoroutine(Enter());
        /*
        if (hasPreviouslyEntered) StartCoroutine(StartMoving());
        else StartCoroutine(Enter());
        */
    }

    private bool DidPassRetreatPoint() => Vector2.Dot(retreatPoint.position - transform.position, moveDir) <= 0;
}
