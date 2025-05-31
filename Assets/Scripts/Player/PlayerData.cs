using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Player Character")]

    [Tooltip("The name of the player character")]
    public string characterName;

    [Tooltip("The ID of the player character")]
    public CharID characterID;


    [Header("Horizontal Movement")]

    public float movementSpeed;


    [Header("Falling")]

    [Tooltip("The downward vertical acceleration applied when in the air")]
    public float gravity;

    [Tooltip("The maximum fall velocity")]
    public float terminalVelocity;

    [Tooltip("The maximum fall velocity when sliding")]
    public float wallSlideTerminalVelocity = 10f;

    [Tooltip("The minimum angle in degrees a slope must make to the ground to cause slipping")]
    public float slipAngle = 45f;


    [Header("Acceleration")]

    [Tooltip("The time it takes to accelerate to maximum horizontal speed from rest on the ground")]
    public float groundAccelerationTime;

    [Tooltip("The time it takes to decelerate to rest from maximum horizontal speed on the ground")]
    public float groundDecelerationTime;

    [Tooltip("The time it takes to accelerate to maximum horizontal speed from rest in the air")]
    public float airAccelerationTime;

    [Tooltip("The time it takes to decelerate to rest by moving from maximum horizontal speed in the air")]
    public float airMovementDecelerationTime;

    [Tooltip("The time it takes to decelerate to rest via drag from maximum horizontal speed in the air")]
    public float airDragDecelerationTime;


    [Header("Jump")]

    [Tooltip("The height in tiles/units of a jump")]
    public float jumpHeight = 2;

    [Tooltip("The factor by which the velocity of a normal jump is scaled by for a double jump")]
    public float DoubleJumpFactor = 0.5f;

    [Tooltip("The amount of extra time the player has to jump after leaving the ground")]
    public float coyoteTime;

    [Tooltip("The amount of time the player has to buffer a jump input")]
    public float jumpBufferTime;

    [Tooltip("Multiplier applied to the player's gravity when they are falling")]
    public float fallingGravityMultiplier;


    [Header("Wall Jump")]

    [Tooltip("The amount of time that wall jumping locks the player out of movement")]
    public float wallJumpMoveLockTime;

    [Tooltip("The player's horizontal speed is set to this value on wall jump")]
    public float wallJumpHorizontalSpeed;

    [Tooltip("The factor by which the player's jump velocity is scaled for a wall jump")]
    public float wallJumpHeightScale = 1.5f;
}
