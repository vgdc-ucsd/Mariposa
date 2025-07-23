using Unity.VisualScripting;
using UnityEngine;

public class Robot : Enemy
{
    [HideInInspector] public RobotMovement Movement;
    private Animator animationController;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float closeDistance = 1f;



    protected override void Awake()
    {
        base.Awake();
        Movement = GetComponent<RobotMovement>();
        animationController = this.gameObject.GetComponent<Animator>();
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Player.ActivePlayer.gameObject)
        {
            StartCoroutine(Player.ActivePlayer.Die());
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 targetPos = Player.ActivePlayer.transform.position;
        if (targetPos.x - transform.position.x > closeDistance)
        {
            Movement.SetMoveDir(Vector2.right);
            animationController.SetBool("IsMoving", true);
            spriteRenderer.flipX = false;

        }
        else if (transform.position.x - targetPos.x > closeDistance)
        {
            Movement.SetMoveDir(Vector2.left);
            animationController.SetBool("IsMoving", true);
            spriteRenderer.flipX = true;
        }
        else
        {
            Movement.SetMoveDir(Vector2.zero);
            animationController.SetBool("IsMoving", false);
        }
    }
}
