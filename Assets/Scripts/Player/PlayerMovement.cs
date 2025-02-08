using NUnit.Framework.Internal.Commands;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerMovement : FreeBody
{
    public static PlayerMovement Instance;

    [Header("Horizontal Parameters")]

    [Tooltip("The maximum horizontal movement speed")]
    public float MoveSpeed = 10;

    

    [Tooltip("The time it takes to accelerate to maximum horizontal speed from rest on the ground")]
    [SerializeField] private float groundAccelerationTime;
    private float groundAcceleration;

    [Tooltip("The time it takes to decelerate to rest from maximum horizontal speed on the ground")]
    [SerializeField] private float groundDecelerationTime;
    private float groundDeceleration;

    [Tooltip("The time it takes to accelerate to maximum horizontal speed from rest in the air")]
    [SerializeField] private float airAccelerationTime;
    private float airAcceleration;

    [Tooltip("The time it takes to decelerate to rest by moving from maximum horizontal speed in the air")]
    [SerializeField] private float airMovementDecelerationTime;
    private float airMovementDeceleration;

    [Tooltip("The time it takes to decelerate to rest via drag from maximum horizontal speed in the air")]
    [SerializeField] private float airDragDecelerationTime;
    private float airDragDeceleration;

    [Header("Vertical Parameters")]
    [Tooltip("Vertical speed is set to this value on jump")]
    public float JumpSpeed = 6; // todo: change to define height

    [Tooltip("The amount of extra time the player has to jump after leaving the ground")]
    [SerializeField] private float coyoteTime;
    private float coyoteTimeRemaining = 0.0f;

    [Tooltip("The amount of time the player has to buffer a jump input")]
    [SerializeField] private float jumpBufferTime;
    private float jumpBufferTimeRemaining = 0.0f;

    [Tooltip("The maximum horizontal distance by which the player's collider is inset the barrier's collider for corner correction")]
    [SerializeField] private float cornerCorrectMaxInsetDistance;

    [Tooltip("The minimum upward velocity required to corner correct")]
    [SerializeField] private float cornerCorrectMinVelocity;


    // Useful for when their dependent values are changed during runtime
    private void InitDerivedConsts()
    {
        groundAcceleration = MoveSpeed / groundAccelerationTime;
        groundDeceleration = MoveSpeed / groundDecelerationTime;
        airAcceleration = MoveSpeed / airAccelerationTime;
        airMovementDeceleration = MoveSpeed / airMovementDecelerationTime;
        airDragDeceleration = MoveSpeed / airDragDecelerationTime;
    }

    protected override void Awake()
    {
        base.Awake();

        InitDerivedConsts();
        if (Instance == null)
        {
            Instance = this;
        }
        
    }

    protected override void Update()
    {
        base.Update();

        // All "Down" inputs should be in Update() to avoid inputs dropping between physics frames
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    private void OnValidate()
    {
        InitDerivedConsts();
    }

    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Input.GetKey(KeyCode.D)) Move(1);
        else if (Input.GetKey(KeyCode.A)) Move(-1);
        else Move(0);

        UpdateTimers(fdt);
    }

    // Move by player input: 1 = right, -1 = left, 0 = none
    private void Move(int dir)
    {
        // Check which acceleration parameter to use
        float accelerationParam = (dir * Velocity.x > 0)
            ? (State == BodyState.InAir)
                ? airAcceleration
                : groundAcceleration
            : (State == BodyState.InAir)
                ? (dir == 0)
                    ? airDragDeceleration
                    : airMovementDeceleration
                : groundDeceleration;
        // Compute the difference between the max velocity and the current velocity
        float deltaV = dir * MoveSpeed - Velocity.x;
        // If the acceleration parameter would cause the player to exceed their maximum speed,
        // use enough acceleration to reach maximum speed. Otherwise, use the acceleration parameter
        float acceleration = Mathf.Min(Mathf.Abs(deltaV / fdt), accelerationParam);

        // Apply the acceleration
        Velocity.x += acceleration * fdt * Mathf.Sign(deltaV);
    }


    // Directly set the player's y velocity
    private void Jump()
    {
        if (State == BodyState.OnGround || coyoteTimeRemaining > 0f)
        {
            Velocity.y = JumpSpeed;
            StartFalling();
            coyoteTimeRemaining = 0f;   // consume coyote time
        }
        else if (State != BodyState.OnGround && coyoteTimeRemaining <= 0.0f)
        {
            jumpBufferTimeRemaining = jumpBufferTime;
            return;
        }

    }

    protected override bool Land(RaycastHit2D hit)
    {
        if (base.Land(hit))
        {
            if (jumpBufferTimeRemaining > 0.0f) Jump();
            return true;
        }
        return false;
    }

    protected override bool StartFalling()
    {
        if (base.StartFalling())
        {
            coyoteTimeRemaining = coyoteTime;
            return true;
        }
        return false;
    }

    // Updates the time of all movement-related timers
    private void UpdateTimers(float dt)
    {
        if (coyoteTimeRemaining > 0) coyoteTimeRemaining -= dt;
        if (jumpBufferTimeRemaining > 0) jumpBufferTimeRemaining -= dt;
    }

    // This override is just for corner correction
    protected override RaycastHit2D CheckTouchingCeiling()
    {
        var ceilCast = base.CheckTouchingCeiling();

        if (!ceilCast) return ceilCast;
        if (Velocity.y < cornerCorrectMinVelocity) return ceilCast;

        float leftInsetDistance = Mathf.Abs(SurfaceCollider.bounds.max.x - ceilCast.collider.bounds.min.x);
        if (leftInsetDistance < cornerCorrectMaxInsetDistance)
        {
            transform.position += leftInsetDistance * Vector3.left;
        }

        float rightInsetDistance = Mathf.Abs(SurfaceCollider.bounds.min.x - ceilCast.collider.bounds.max.x);
        if (rightInsetDistance < cornerCorrectMaxInsetDistance)
        {
            transform.position += rightInsetDistance * Vector3.right;
        }

        return ceilCast;
    }
}
