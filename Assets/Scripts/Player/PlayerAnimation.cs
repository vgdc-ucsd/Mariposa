using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    SpriteRenderer playerSprite;
    Animator animator;
    [SerializeField] String TerrainTag = "Default";
    [SerializeField] String FootstepSoundEvent = "event:/sfx/player/footstep";

    Boolean WasRunning;
    void Start()
    {
        animator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        // at rest
        if (Player.ActivePlayer.Movement.Velocity.sqrMagnitude <= 0.05f)
        {
            if (WasRunning == true)
            {
                PlayFootstep();
                WasRunning = false;
            }
            animator.SetFloat("xVelocity", 0);
            return;
        }

        int dir = Player.ActivePlayer.FacingDirection;
        if (dir == -1)
        {
            animator.SetFloat("xVelocity", 1);
            playerSprite.flipX = true;
            WasRunning = true;
        }
        else
        {
            animator.SetFloat("xVelocity", 1);
            playerSprite.flipX = false;
            WasRunning = true;
        }
    }

    String MaterialCheck()
    {
        //TODO: need to be able to check what is on the ground
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
        EventInstance footstepInstance = RuntimeManager.CreateInstance(FootstepSoundEvent);
        footstepInstance.setParameterByNameWithLabel("Terrain", MaterialCheck());
        footstepInstance.start();
        footstepInstance.release();
    }
}
