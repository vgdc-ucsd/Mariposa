using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // SpriteRenderer playerSprite;
    Animator animator;
    [SerializeField] string TerrainTag = "Default";
    // NOTE: jump sfx is being handled by PlayerMovement.cs

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Jumping
        bool isJumping = Player.ActivePlayer.Movement.State == BodyState.InAir;
        animator.SetBool("isJumping", isJumping);
    }

    void FixedUpdate()
    {
        // Idle, not moving
        if (Player.ActivePlayer.Movement.Velocity.sqrMagnitude <= 0.005f)
        {
            StartCoroutine(IdleDelay(() => animator.SetFloat("xVelocity", 0f)));
            animator.SetBool("isJumping", false);
            animator.SetFloat("yVelocity", 0f);
        }
        // Running
        else
        {
            StopAllCoroutines();
            animator.SetFloat("xVelocity", 1);
            animator.SetFloat("yVelocity", Player.ActivePlayer.Movement.Velocity.y);

            int dir = Player.ActivePlayer.FacingDirection;
            if (dir == -1)
            {
                animator.SetFloat("faceLeft", 1);
            }
            else
            {
                animator.SetFloat("faceLeft", 0);
            }
        }
    }

    IEnumerator IdleDelay(Action idle)
    {
        yield return new WaitForSeconds(0.03f);
        idle();
    }

    private string MaterialCheck()
    {
        //TODO: need to be able to check what is on the ground
        // will be held off until later implementation of terrain checks
        switch (TerrainTag)
        {
            case "Concrete":
                return "Concrete";
            case "Wood":
                return "Wood";
            default:
                return "Default";
        }
    }

    // function is called in the animator to match visual to audio
    public void PlayFootstep()
    {
        if (Player.ActivePlayer.Movement.State == BodyState.OnGround)
        {
            EventInstance footstepInstance = AudioManager.CreateEventInstance(AudioEvents.SFX.player_footstep);
            footstepInstance.setParameterByNameWithLabel("Terrain", MaterialCheck());
            footstepInstance.start();
            footstepInstance.release();
        }
    }
}
