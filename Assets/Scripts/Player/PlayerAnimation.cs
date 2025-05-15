using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator animator;
    [SerializeField] string TerrainTag = "Default";
    // NOTE: jump sfx is being handled by PlayerMovement.cs

    private bool willLand = false;
    private const float LANDING_VELOCITY_THRESHOLD = -16.0f; // set slightly above the normal velocity when jumping and falling in place (aprox. -14.6)
    // TODO: should it be set so that jump height triggers landing sfx or slightly above or below it

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Player.ActivePlayer.Movement.State == BodyState.InAir)
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
        PlayLand();
        // at rest
        if (Player.ActivePlayer.Movement.Velocity.sqrMagnitude <= 0.05f)
        {
            animator.SetBool("isJumping", false);
            animator.SetFloat("xVelocity", 0f);
            animator.SetFloat("yVelocity", 0f);
            return;
        }

        int dir = Player.ActivePlayer.FacingDirection;
        if (dir == -1)
        {
            animator.SetFloat("xVelocity", 1);
            animator.SetFloat("yVelocity", Player.ActivePlayer.Movement.Velocity.y);
            animator.SetFloat("faceLeft", 1);
        }
        else
        {
            animator.SetFloat("xVelocity", 1);
            animator.SetFloat("yVelocity", Player.ActivePlayer.Movement.Velocity.y);
            animator.SetFloat("faceLeft", 0);
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

    // function is called using the animator to match visual to audio
    private void PlayFootstep()
    {
        if (Player.ActivePlayer.Movement.State == BodyState.OnGround && !willLand)
        {
            EventInstance footstepInstance = RuntimeManager.CreateInstance(AudioEvents.SFX.player_footstep.GetPath());
            footstepInstance.setParameterByNameWithLabel("Terrain", MaterialCheck());
            footstepInstance.start();
            footstepInstance.release();
        }
    }

    private void PlayLand()
    {
        if (Player.ActivePlayer.Movement.State == BodyState.InAir && Player.ActivePlayer.Movement.Velocity.y <= LANDING_VELOCITY_THRESHOLD)
        {
            //Debug.Log(Player.ActivePlayer.Movement.Velocity.y);
            willLand = true;
        }
        else if (Player.ActivePlayer.Movement.State == BodyState.OnGround && willLand)
        {
            RuntimeManager.PlayOneShot(AudioEvents.SFX.player_landing.GetPath());
            willLand = false;
        }
    }
}