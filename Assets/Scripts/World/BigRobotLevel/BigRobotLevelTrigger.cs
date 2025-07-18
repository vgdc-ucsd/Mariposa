using UnityEngine;

public class BigRobotLevelTrigger : MonoBehaviour
{
    [SerializeField] private BigRobotLevel.CurrentSection sectionToChangeTo;
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
            BigRobotLevel.Instance.TriggerNextSection(sectionToChangeTo);
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
