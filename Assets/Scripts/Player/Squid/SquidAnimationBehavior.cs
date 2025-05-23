using UnityEngine;

public class SquidAnimationBehavior : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sRenderer;
    // [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    bool wasRunning;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        sRenderer = GetComponent<SpriteRenderer>();
        animator.SetBool("isJumping", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (SquidMovement.Instance.State == BodyState.InAir)
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }
    }

    void FixedUpdate()
    {
        if (SquidMovement.Instance.Velocity.sqrMagnitude <= 0.05f)
        {
            if (wasRunning && Player.ActivePlayer.Movement.State == BodyState.OnGround)
            {
                animator.SetBool("isMoving", false);
            }
            animator.SetBool("isJumping", false);
            animator.SetFloat("xVelocity", 0f);
            animator.SetFloat("yVelocity", 0f);
            wasRunning = false;
            return;
        }

        if (Player.ActivePlayer.FacingDirection == -1)
        {
            animator.SetFloat("xVel", 1);
            animator.SetFloat("yVel", Player.ActivePlayer.Movement.Velocity.y);
            animator.SetFloat("faceRight", 0);
            sRenderer.flipX = true;
            animator.SetBool("isMoving", true);
            wasRunning = true;
        }
        else
        {
            animator.SetFloat("xVel", 1);
            animator.SetFloat("yVel", Player.ActivePlayer.Movement.Velocity.y);
            animator.SetFloat("faceRight", 1);
            sRenderer.flipX = false;
            animator.SetBool("isMoving", true);
            wasRunning = true;
        }
    }
}
