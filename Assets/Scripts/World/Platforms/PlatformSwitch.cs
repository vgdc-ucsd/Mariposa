using FMODUnity;
using System;
using UnityEngine;

public class PlatformSwitch : Switch
{
    [SerializeField] private SpriteRenderer sr;
    [Serializable] class PlatformMovement
    {
        public SendableMovingPlatform platform;

        [Tooltip("the number of nodes the platform will move by")]
        public int nodesToMove = 1;
    }

    [SerializeField] private PlatformMovement[] platformMovements;

    public override void TriggerSwitch()
    {
        sr.flipX = !sr.flipX;
        foreach (PlatformMovement platformMovement in platformMovements)
        {
            platformMovement.platform.SendPlatform(platformMovement.nodesToMove);
        }
        RuntimeManager.PlayOneShot(AudioEvents.SFX.lever_pull);
    }
}