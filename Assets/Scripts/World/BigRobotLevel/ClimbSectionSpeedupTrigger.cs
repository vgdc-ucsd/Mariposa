using UnityEngine;

public class ClimbSectionSpeedupTrigger : MonoBehaviour
{
    [SerializeField] private ChaseRobot robotToSpeedup;
    [SerializeField] private float speedupFactor = 1.0f;
    [SerializeField] private Vector2 newStartPos;
    [SerializeField] private float newEnterOffset;
    [SerializeField] private float newEnterTime;
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        ResetTrigger();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            robotToSpeedup.baseMovementSpeed *= speedupFactor;
            robotToSpeedup.startPos = newStartPos;
            robotToSpeedup.timeToEnter = newEnterTime;
            robotToSpeedup.enterStartOffset = newEnterOffset;
            robotToSpeedup.movementStartDelay = 0.0f;
            col.enabled = false;
        }
    }

    private void OnEnable()
    {
        Player.OnDeath += ResetTrigger;
    }

    private void OnDisable()
    {
        Player.OnDeath -= ResetTrigger;
    }

    private void ResetTrigger()
    {
        col.enabled = true;
    }
}
