using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    SpriteRenderer playerSprite;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            animator.SetFloat("xVelocity", 1);
            playerSprite.flipX = true;
        }

        else if (Input.GetKey(KeyCode.D))
        {
            animator.SetFloat("xVelocity", 1);
            playerSprite.flipX = false;
        }
        else 
        {
            animator.SetFloat("xVelocity", 0);
        }
    }
}
