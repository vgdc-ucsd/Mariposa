using Unity.VisualScripting;
using UnityEngine;

public class Robot : Enemy
{
    [HideInInspector] public RobotMovement Movement;

    [SerializeField] private float closeDistance = 1f;
    


    protected override void Awake()
    {
        base.Awake();
        Movement = GetComponent<RobotMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Player.ActivePlayer.gameObject)
        {
            Player.ActivePlayer.Respawn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 targetPos = Player.ActivePlayer.transform.position;
        if (targetPos.x - transform.position.x > closeDistance)
        {
            Movement.SetMoveDir(Vector2.right);
        }
        else if (transform.position.x - targetPos.x > closeDistance)
        {
            Movement.SetMoveDir(Vector2.left);
        }
        else
        {
            Movement.SetMoveDir(Vector2.zero);
        }
    }
}
