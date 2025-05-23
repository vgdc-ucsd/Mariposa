using FMODUnity;
using NUnit.Framework.Internal.Commands;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SquidMovement : FreeBody, IInputListener, IControllable
{
    public static SquidMovement Instance;


    private Vector2 moveDir = Vector2.zero;
    private bool onWalkableSlope = false;
    private Vector2 slopeDir = Vector2.zero;

    private int wallNormal = 0; // -1 = left, 1 = right, 0 = not on a wall


    [SerializeField] private PlayerData data;


    private float groundAcceleration;
    private float groundDeceleration;
    private float airAcceleration;
    private float airMovementDeceleration;
    private float airDragDeceleration;
    private float jumpVelocity;

    private float coyoteTimeRemaining = 0.0f;
    private float jumpBufferTimeRemaining = 0.0f;

    [Tooltip("The maximum horizontal distance by which the player's collider is inset the barrier's collider for corner correction")]
    [SerializeField] private float cornerCorrectMaxInsetDistance;
    [Tooltip("The minimum upward velocity required to corner correct")]
    [SerializeField] private float cornerCorrectMinVelocity;



    SpriteRenderer beeSprite;

    // public bool IsControlled;

    // only activate triggers when it is being controlled

    public override bool ActivateTriggers => (IControllable)this == PlayerController.Instance.CurrentControllable;

    private void InitDerivedConsts()
    {
        // Gravity = data.gravity;
        TerminalVelocity = data.terminalVelocity;
        slipAngle = data.slipAngle;

        groundAcceleration = data.movementSpeed / data.groundAccelerationTime;
        groundDeceleration = data.movementSpeed / data.groundDecelerationTime;
        airAcceleration = data.movementSpeed / data.airAccelerationTime;
        airMovementDeceleration = data.movementSpeed / data.airMovementDecelerationTime;
        airDragDeceleration = data.movementSpeed / data.airDragDecelerationTime;

        jumpVelocity = Mathf.Sqrt(2 * Gravity * data.jumpHeight);
    }

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
        InitDerivedConsts();
        beeSprite = GetComponent<SpriteRenderer>();

    }

    private void Start()
    {
        collisionLayer = LayerMask.GetMask("SquidBarrier");
    }

    protected override void Update()
    {
        base.Update();

    }

    private void OnValidate()
    {
        InitDerivedConsts();
    }


    protected override void FixedUpdate()
    {
        Move();

        base.FixedUpdate();
        CeilingCornerCorrect();
        CheckOnWall();

        UpdateTimers(fdt);
    }


    private void CheckOnWall()
    {
        Bounds bounds = SurfaceCollider.bounds;
        RaycastHit2D leftHit = Physics2D.BoxCast(bounds.center, bounds.size, 0f, Vector2.left, COLLISION_CHECK_DISTANCE, collisionLayer);
        RaycastHit2D rightHit = Physics2D.BoxCast(bounds.center, bounds.size, 0f, Vector2.right, COLLISION_CHECK_DISTANCE, collisionLayer);
        bool hitLeftWall = Mathf.Abs(leftHit.normal.normalized.x) > LAND_SLOPE_FACTOR;
        bool hitRightWall = Mathf.Abs(rightHit.normal.normalized.x) > LAND_SLOPE_FACTOR;

        if (hitLeftWall) wallNormal = 1;
        else if (hitRightWall) wallNormal = -1;
        else wallNormal = 0;
    }

    // public method to send a move command
    public void SetMoveDir(Vector2 dir)
    {
        moveDir = dir;
        if (!Mathf.Approximately(dir.x, 0f)) Player.ActivePlayer.TurnTowards((int)Mathf.Sign(dir.x));
    }

    private void Move()
    {
        int dir = Mathf.RoundToInt(moveDir.x);

        //if (dir != 0) Player.ActivePlayer.FacingDirection = dir;


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
        float deltaV = dir * data.movementSpeed - Velocity.x;
        // If the acceleration parameter would cause the player to exceed their maximum speed,
        // use enough acceleration to reach maximum speed. Otherwise, use the acceleration parameter
        float acceleration = Mathf.Min(Mathf.Abs(deltaV / fdt), accelerationParam);

        Vector2 axis = onWalkableSlope ? slopeDir : Vector2.right;

        // Apply the acceleration
        Velocity += acceleration * fdt * Mathf.Sign(deltaV) * axis;
    }

    // Directly set the player's y velocity
    public void JumpInputDown()
    {
        Physics2D.SyncTransforms();
        Bounds bounds = SurfaceCollider.bounds;

      
        if (State == BodyState.OnGround || coyoteTimeRemaining > 0f)
        {
            Velocity.y = jumpVelocity;
            coyoteTimeRemaining = 0f;   // consume coyote time
            RuntimeManager.PlayOneShot("event:/sfx/player/jump");
        }
        else if (State != BodyState.OnGround && coyoteTimeRemaining <= 0.0f)
        {
            jumpBufferTimeRemaining = data.jumpBufferTime;
        }
    }

    // This override makes the player fall slower/faster when falling
    protected override void Fall()
    {
        if (State != BodyState.InAir || !gravityEnabled) return;

        float currentGravity = Velocity.y > 0 ? Gravity : Gravity * data.fallingGravityMultiplier;
        Velocity.y = Mathf.Max(Velocity.y - currentGravity * fdt, -TerminalVelocity);
    }

    protected override void OnGrounded(RaycastHit2D groundHit)
    {
        base.OnGrounded(groundHit);
        if (jumpBufferTimeRemaining > 0.0f) JumpInputDown();
        if (Mathf.Abs(Mathf.Atan(groundHit.normal.y / groundHit.normal.x) * Mathf.Rad2Deg) > slipAngle)
        {
            onWalkableSlope = true;
            slopeDir = new Vector2(groundHit.normal.normalized.y, -groundHit.normal.normalized.x);
        }
    }

    protected override void OnAirborne()
    {
        if (State == BodyState.OnGround)
        {
            coyoteTimeRemaining = data.coyoteTime;
        }
        base.OnAirborne();
        onWalkableSlope = false;
        slopeDir = Vector2.zero;
    }

    // Shift the player horizontally when they barely bump a ceiling
    private void CeilingCornerCorrect()
    {
        RaycastHit2D ceilingHit = collisionHits.Where(hit => hit.normal.normalized.y < -LAND_SLOPE_FACTOR).FirstOrDefault();

        if (!ceilingHit) return;
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
        coyoteTimeRemaining = Mathf.Max(coyoteTimeRemaining - dt, 0.0f);
        jumpBufferTimeRemaining = Mathf.Max(jumpBufferTimeRemaining - dt, 0.0f);
    }

    public void ToggleCollisions(bool toggle)
    {
        collisionsEnabled = toggle;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }


}