using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // SpriteRenderer playerSprite;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        // playerSprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        // at rest
        if (Player.ActivePlayer.Movement.Velocity.sqrMagnitude <= 0.05f)
        {
            animator.SetFloat("xVelocity", 0);
            return;
        }

        int dir = Player.ActivePlayer.FacingDirection;
        if (dir == -1)
        {
            animator.SetFloat("xVelocity", 1);
            animator.SetFloat("faceLeft", 1);
        }
        else
        {
            animator.SetFloat("xVelocity", 1);
            animator.SetFloat("faceLeft", 0);
        }
    }
}
