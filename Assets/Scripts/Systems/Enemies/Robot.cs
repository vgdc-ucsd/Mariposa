using UnityEngine;

public class Robot : MonoBehaviour
{
    [HideInInspector] public RobotMovement Movement;

    [SerializeField] private float closeDistance = 1f;
    
    private void Awake()
    {
        Movement = GetComponent<RobotMovement>();
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
