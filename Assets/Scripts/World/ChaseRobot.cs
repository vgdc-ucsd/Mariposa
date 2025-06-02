using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class ChaseRobot : MonoBehaviour
{
    public enum RobotWallState
    {
        IDLE,
        MOVING
    }

    [Header("Params")]
    [SerializeField] private float movementStartDelay = 1.0f;
    [SerializeField] private float catchupDistanceThreshold = 5.0f;
    [SerializeField] private float maxSpeed = 40.0f;
    [SerializeField] private float catchupRate = 5.0f;

    [Header("State")]
    public Vector2 moveDir;
    public float baseMovementSpeed = 4.0f;
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
        }
        Vector2 targetPos = Camera.main.transform.position;
        Vector2 separationToTarget = (Vector2)transform.position - targetPos;
        Vector3 correction = separationToTarget - Helper.Vec2Proj(separationToTarget, moveDir);
        transform.position -= correction;
    }

    public void ResetRobot()
    {
        transform.position = startPos;
        currentSpeed = baseMovementSpeed;

        StartCoroutine(StartMoving());
    }
}
