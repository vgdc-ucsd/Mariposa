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
    public float JumpHeight = 6;

    [Tooltip("Checks if Player can use Double Jump")]
    public bool CanDoubleJump = true; 

    [Tooltip("The amount of extra time the player has to jump after leaving the ground")]
    [SerializeField] private float coyoteTime;
    private float coyoteTimeRemaining = 0.0f;

    [Tooltip("The amount of time the player has to buffer a jump input")]
    [SerializeField] private float jumpBufferTime;
    private float jumpBufferTimeRemaining = 0.0f;

    [Tooltip("Multiplier applied to the player's gravity when they are falling")]
    [SerializeField] private float fallingGravityMultiplier;

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
        if (Input.GetKeyDown(KeyCode.Space))Jump();
    }

    private void OnValidate()
    {
        InitDerivedConsts();
    }

    
    protected override void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.D)) Move(1);
        else if (Input.GetKey(KeyCode.A)) Move(-1);
        else Move(0);

        CeilingCornerCorrect();
        base.FixedUpdate();

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
            Velocity.y = JumpHeight;
            coyoteTimeRemaining = 0f;   // consume coyote time
        }
        else if(CanDoubleJump){
            Velocity.y = 1.25f * JumpHeight;
            CanDoubleJump = false;
            coyoteTimeRemaining = 0f;
        }
        else if (State != BodyState.OnGround && coyoteTimeRemaining <= 0.0f)
        {
            jumpBufferTimeRemaining = jumpBufferTime;
        }
    }


    // This override makes the player fall slower/faster when falling
    protected override void Fall()
    {
        if (State != BodyState.InAir) return;

        float currentGravity = Velocity.y > 0 ? Gravity : Gravity * fallingGravityMultiplier;
        Velocity.y = Mathf.Max(Velocity.y - currentGravity * fdt, -TerminalVelocity);
    }

    // If the player was in the air and has a barrier below it, they must now be on the ground
    // Otherwise, if the player was grounded and has no barrier below it, they must now be in the air
    protected override void CheckGrounded()
    {
        Bounds bounds = SurfaceCollider.bounds;
        RaycastHit2D groundHit = Physics2D.BoxCast(bounds.center, bounds.size, 0f, Vector2.down, COLLISION_CHECK_DISTANCE, collisionLayer);
        if (State != BodyState.OnGround && groundHit && groundHit.normal.normalized.y > LAND_SLOPE_FACTOR)
        {
            State = BodyState.OnGround;
            CanDoubleJump = true;
            if (jumpBufferTimeRemaining > 0.0f) Jump();
        }
        else if (State == BodyState.OnGround && !groundHit)
        {
            State = BodyState.InAir;
            coyoteTimeRemaining = coyoteTime;
        }
    }

    // Shift the player horizontally when they barely bump a ceiling
    private void CeilingCornerCorrect()
    {
        Bounds bounds = SurfaceCollider.bounds;
        RaycastHit2D ceilingHit = Physics2D.BoxCast(bounds.center, bounds.size, 0f, Vector2.up, COLLISION_CHECK_DISTANCE, collisionLayer);

        if (!ceilingHit || ceilingHit.normal.normalized.y > -LAND_SLOPE_FACTOR) return;
        if (Velocity.y < cornerCorrectMinVelocity) return;

        float leftInsetDistance = Mathf.Abs(SurfaceCollider.bounds.max.x - ceilingHit.collider.bounds.min.x);
        if (leftInsetDistance < cornerCorrectMaxInsetDistance)
        {
            transform.position += (leftInsetDistance + CONTACT_OFFSET) * Vector3.left;
        }

        float rightInsetDistance = Mathf.Abs(SurfaceCollider.bounds.min.x - ceilingHit.collider.bounds.max.x);
        if (rightInsetDistance < cornerCorrectMaxInsetDistance)
        {
            transform.position += (rightInsetDistance + CONTACT_OFFSET) * Vector3.right;
        }
    }

    // Updates the time of all movement-related timers
    private void UpdateTimers(float dt)
    {
        if (coyoteTimeRemaining > 0) coyoteTimeRemaining -= dt;
        if (jumpBufferTimeRemaining > 0) jumpBufferTimeRemaining -= dt;
    }
}
