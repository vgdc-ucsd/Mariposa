using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // SpriteRenderer playerSprite;
    Animator animator;
    [SerializeField] String TerrainTag = "Default";
    [SerializeField] String FootstepSoundEvent = "event:/sfx/player/footstep";
    [SerializeField] String JumpSoundEvent = "event:/sfx/player/jump";
    [SerializeField] String LandSoundEvent = "event:/sfx/player/land";

    Boolean WasRunning;

    void Start()
    {
        animator = GetComponent<Animator>();
        // playerSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(Player.ActivePlayer.Movement.State == BodyState.InAir) {
            animator.SetBool("isJumping", true);
        } else {
            animator.SetBool("isJumping", false);
        }
    }

    void FixedUpdate()
    {
        // at rest
        if (Player.ActivePlayer.Movement.Velocity.sqrMagnitude <= 0.05f)
        {
            if (WasRunning == true && Player.ActivePlayer.Movement.State == BodyState.OnGround)
            {
                PlayFootstep();
                WasRunning = false;
            }
            animator.SetBool("isJumping", false);
            animator.SetFloat("xVelocity", 0);
            return;
        }

        int dir = Player.ActivePlayer.FacingDirection;
        if (dir == -1)
        {
            animator.SetFloat("xVelocity", 1);
            animator.SetFloat("yVelocity", Player.ActivePlayer.Movement.Velocity.y);
            animator.SetFloat("faceLeft", 1);
            WasRunning = true;
        }
        else
        {
            animator.SetFloat("xVelocity", 1);
            animator.SetFloat("yVelocity", Player.ActivePlayer.Movement.Velocity.y);
            animator.SetFloat("faceLeft", 0);
            WasRunning = true;
        }

        // TODO: Landing is currently playing at the wrong time, waiting on jump animation to be implemented to time with animation change
        if (Player.ActivePlayer.Movement.State == BodyState.OnGround && Player.ActivePlayer.Movement.Velocity.y < -5)    // if air->ground, play landing sound
        {
            //PlayLand();
        }
    }

    String MaterialCheck()
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

    public void PlayFootstep()
    {
        if (Player.ActivePlayer.Movement.State == BodyState.OnGround)
        {
            EventInstance footstepInstance = RuntimeManager.CreateInstance(FootstepSoundEvent);
            footstepInstance.setParameterByNameWithLabel("Terrain", MaterialCheck());
            footstepInstance.start();
            footstepInstance.release();
        }
    }

    public void PlayLand()
    {
        EventInstance footstepInstance = RuntimeManager.CreateInstance(LandSoundEvent);
        footstepInstance.start();
        footstepInstance.release();
    }
}