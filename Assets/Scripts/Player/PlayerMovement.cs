using NUnit.Framework.Internal.Commands;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    private Rigidbody2D rb;
    private Collider2D col;
    [SerializeField] private LayerMask playerLayer;

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

    [Tooltip("The maximum fall velocity")]
    [SerializeField] private float TerminalVelocity;

    [Tooltip("The amount of extra time the player has to jump after leaving the ground")]
    [SerializeField] private float coyoteTime;
    private float coyoteTimeRemaining = 0.0f;

    [Tooltip("The amount of time the player has to buffer a jump input")]
    [SerializeField] private float jumpBufferTime;
    private float jumpBufferTimeRemaining = 0.0f;

    [Tooltip("The downward vertical acceleration applied when in the air")]
    public float Gravity;

    private const float COLLISION_CHECK_DISTANCE = 0.05f; // how far away you have to be from a surface to be considered "colliding" with it
    private const float LAND_SLOPE_FACTOR = 0.9f; // magnitude of the y component of a surface normal to be considered vertical

    [System.NonSerialized] public BodyState State;

    private Vector2 velocity; // the velocity for the current frame
    private float fdt; // Shorthand for fixed delta time

    // Useful for when their dependent values are changed during runtime
    private void InitDerivedConsts()
    {
        groundAcceleration = MoveSpeed / groundAccelerationTime;
        groundDeceleration = MoveSpeed / groundDecelerationTime;
        airAcceleration = MoveSpeed / airAccelerationTime;
        airMovementDeceleration = MoveSpeed / airMovementDecelerationTime;
        airDragDeceleration = MoveSpeed / airDragDecelerationTime;
    }

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        State = BodyState.InAir; // Initialize the player as in the air

        InitDerivedConsts();

        Instance = this;
    }

    private void OnValidate()
    {
        InitDerivedConsts();
    }

    private void FixedUpdate()
    {
        fdt = Time.fixedDeltaTime;

        if (Input.GetKey(KeyCode.D)) Move(1);
        else if (Input.GetKey(KeyCode.A)) Move(-1);
        else Move(0);

        if (Input.GetKey(KeyCode.Space)) Jump();

        Fall();
        CheckGrounded();
        CheckCeilingHit();
        ApplyFriction();

        UpdateVelocity();
        UpdateTimers(fdt);
    }

    // Move by player input: 1 = right, -1 = left, 0 = none
    private void Move(int dir)
    {
        // Check which acceleration parameter to use
        float accelerationParam = (dir * velocity.x > 0)
            ? (State == BodyState.InAir)
                ? airAcceleration
                : groundAcceleration
            : (State == BodyState.InAir)
                ? (dir == 0)
                    ? airDragDeceleration
                    : airMovementDeceleration
                : groundDeceleration;
        // Compute the difference between the max velocity and the current velocity
        float deltaV = dir * MoveSpeed - velocity.x;
        // If the acceleration parameter would cause the player to exceed their maximum speed,
        // use enough acceleration to reach maximum speed. Otherwise, use the acceleration parameter
        float acceleration = Mathf.Min(Mathf.Abs(deltaV / fdt), accelerationParam);

        // Apply the acceleration
        velocity.x += acceleration * fdt * Mathf.Sign(deltaV);
    }

    // Directly set the player's y velocity
    private void Jump()
    {
        if (State != BodyState.OnGround && coyoteTimeRemaining <= 0.0f) {
            jumpBufferTimeRemaining = jumpBufferTime;
            return;
        }

        velocity.y = MoveSpeed; // this should be its own value
    }

    // Apply gravity in the air
    private void Fall()
    {
        if (State != BodyState.InAir) return;

        velocity.y = Mathf.Max(velocity.y - Gravity * fdt, -TerminalVelocity);
    }

    // Check if the player is about to collide with the ground and update the player's bodyState accordingly
    private void CheckGrounded()
    {
        RaycastHit2D groundHit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, COLLISION_CHECK_DISTANCE, ~playerLayer);

        if (State != BodyState.OnGround && groundHit && groundHit.normal.normalized.y > LAND_SLOPE_FACTOR)
        {
            State = BodyState.OnGround;
            velocity.y = 0;
            if (jumpBufferTimeRemaining > 0.0f) Jump();
        }
        else if (State == BodyState.OnGround && !groundHit)
        {
            State = BodyState.InAir;
            coyoteTimeRemaining = coyoteTime;
        }
    }

    // Make the player fall immediately after hitting a ceiling
    private void CheckCeilingHit()
    {
        // Similar raycast to CheckGrounded
        RaycastHit2D ceilingHit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.up, COLLISION_CHECK_DISTANCE, ~playerLayer);

        if (ceilingHit && -ceilingHit.normal.normalized.y > LAND_SLOPE_FACTOR) velocity.y = Mathf.Min(0, velocity.y);
    }

    // Updates the time of all movement-related timers
    private void UpdateTimers(float dt)
    {
        coyoteTimeRemaining -= dt;
        jumpBufferTimeRemaining -= dt;
    }

    private void ApplyFriction()
    {
        if (State == BodyState.OnGround)
        {
            // ...
        }
        else if (State == BodyState.InAir)
        {
            // ...
        }
    }

    // Apply any changes to the velocity to the actual rigidbody velocity
    private void UpdateVelocity() => rb.linearVelocity = velocity;
}
