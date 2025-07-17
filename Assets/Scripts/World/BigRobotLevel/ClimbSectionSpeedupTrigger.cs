using UnityEngine;

public class ClimbSectionSpeedupTrigger : MonoBehaviour
{
    [SerializeField] private ChaseRobot robotToSpeedup;
    [SerializeField] private float speedupFactor = 1.0f;
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
