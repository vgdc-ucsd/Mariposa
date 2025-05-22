using System.Collections;
using NUnit.Framework.Internal.Commands;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using FMODUnity;

public class PlayerMovement : FreeBody, IInputListener, IControllable
{
    public static PlayerMovement Instance;
    public Player Parent;
    public override bool ActivateTriggers => (IControllable)this == PlayerController.Instance.CurrentControllable;

    private PlayerData data;

    private Vector2 moveDir = Vector2.zero;
    private bool onWalkableSlope = false;
    private Vector2 slopeDir = Vector2.zero;

    private int wallNormal = 0; // -1 = left, 1 = right, 0 = not on a wall

    private float groundAcceleration;
    private float groundDeceleration;
    private float airAcceleration;
    private float airMovementDeceleration;
    private float airDragDeceleration;

    private float jumpVelocity;
    [Tooltip("Checks if Player can use Double Jump")]
    public bool CanDoubleJump;
    [Tooltip("Checks if Player can use Wall Jump")]
    public bool CanWallJump;
    private float coyoteTimeRemaining = 0.0f;
    private float jumpBufferTimeRemaining = 0.0f;

    private float wallJumpMoveLockTimeRemaining = 0.0f;

    [Tooltip("The maximum horizontal distance by which the player's collider is inset the barrier's collider for corner correction")]
    [SerializeField] private float cornerCorrectMaxInsetDistance;

    [Tooltip("The minimum upward velocity required to corner correct")]
    [SerializeField] private float cornerCorrectMinVelocity;

    // whether the player has a charge of double jump (whether player touched the ground since last double jump
    public bool airJumpAvailable = false;

    private MovingPlatform currentMovingPlatform = null;
    private bool onControllableMovingPlatform = false;

    // Useful for when their dependent values are changed during runtime
    private void InitDerivedConsts()
    {
        if (Parent == null) Parent = GetComponent<Player>();
        data =  Parent.Data;

        Gravity = data.gravity;
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
        /*
        if (Input.GetKeyDown(KeyCode.V)) // TODO make this an input action
        {
            // Consume a battery if available
            if (InventoryManager.Instance.GetItemCount(InventoryType.Mariposa, BatteryItem.Instance) > 0)
            {
                InventoryManager.Instance.DeleteItem(InventoryType.Mariposa, BatteryItem.Instance);
                StartCoroutine(DashRoutine());
            }
        }
        */
    }

    private void OnValidate()
    {
        if (Application.isPlaying) InitDerivedConsts();
    }

    protected override void FixedUpdate()
    {
        if (onControllableMovingPlatform) ControlPlatform();
        Move();

        base.FixedUpdate();
        CeilingCornerCorrect();
        CheckOnWall();

        UpdateTimers(fdt);
    }

    private void ControlPlatform()
    {
        ControllableMovingPlatform platform = (ControllableMovingPlatform)currentMovingPlatform;
        platform.MovePlatform(moveDir);
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

        if (State == BodyState.InAir && wallNormal != 0 && wallNormal + (int)moveDir.x == 0) 
        {
            Velocity.y = Mathf.Max(Velocity.y, -data.wallSlideTerminalVelocity);
        }
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

        // Lock the player's movement if they wall jumped recently
        if (wallJumpMoveLockTimeRemaining > 0.0f) dir = 0;

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

        bool onBee = false;
        if (CanDoubleJump && airJumpAvailable)
        {
            Vector2 beeCastCenter = bounds.center + 0.375f * bounds.size.y * Vector3.down;
            Vector2 beeCastSize = 0.25f * bounds.size;
            RaycastHit2D[] beeHits = Physics2D.BoxCastAll(beeCastCenter, beeCastSize, 0f, Vector2.down, COLLISION_CHECK_DISTANCE);
            foreach (var hit in beeHits) if (hit.collider.CompareTag("Bee")) onBee = true;
        }

        if (CanWallJump && wallNormal != 0 && State == BodyState.InAir)
        {
            Velocity.y = jumpVelocity * data.wallJumpHeightScale;
            Velocity.x = data.wallJumpHorizontalSpeed * wallNormal;
            wallJumpMoveLockTimeRemaining = data.wallJumpMoveLockTime;
            RuntimeManager.PlayOneShot("event:/sfx/player/jump");
        }
        else if (State == BodyState.OnGround || coyoteTimeRemaining > 0f)
        {
            Velocity.y = jumpVelocity;
            coyoteTimeRemaining = 0f;   // consume coyote time
            RuntimeManager.PlayOneShot("event:/sfx/player/jump");
        }
        else if (CanDoubleJump && airJumpAvailable && Bee.Instance.CanBeeJump())
        {
            StartCoroutine(DelayedJump());
            Bee.Instance.TriggerBeeJump();

        }
        else if (State != BodyState.OnGround && coyoteTimeRemaining <= 0.0f)
        {
            jumpBufferTimeRemaining = data.jumpBufferTime;
        }
    }

    private IEnumerator DelayedJump()
    {
        yield return new WaitForSeconds(0.15f);
        Velocity.y = jumpVelocity * data.DoubleJumpFactor;
        airJumpAvailable = false;
        coyoteTimeRemaining = 0f;
        RuntimeManager.PlayOneShot("event:/sfx/player/jump");
        RuntimeManager.PlayOneShot("event:/sfx/player/bee/double_jump");
        
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
        if (State != BodyState.OnGround)
        {
            if (groundHit.collider.CompareTag("MovingPlatform"))
            {
                currentMovingPlatform = groundHit.collider.GetComponentInParent<MovingPlatform>();
                //if (currentMovingPlatform.currentMovement.y < 0) 
                    //transform.position += currentMovingPlatform.currentMovement.y * Vector3.up;
                currentMovingPlatform.adjacentFreeBody = this;
                if (currentMovingPlatform is ControllableMovingPlatform) onControllableMovingPlatform = true;
            }
            airJumpAvailable = true;
        }
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
        if (onControllableMovingPlatform)
        {
            currentMovingPlatform.currentMovement = Vector2.zero;
            ((ControllableMovingPlatform)currentMovingPlatform).MovePlatform(Vector2.zero);
        }
        if (currentMovingPlatform != null) currentMovingPlatform.adjacentFreeBody = null;
        currentMovingPlatform = null;
        onControllableMovingPlatform = false;
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
        wallJumpMoveLockTimeRemaining = Mathf.Max(wallJumpMoveLockTimeRemaining - dt, 0.0f);
    }

    /*
    /// <summary>
    /// Coroutine that handles the dash ability.
    /// </summary>
    private IEnumerator DashRoutine()
    {
        isDashing = true;
        float dashDirection = Mathf.Sign(transform.localScale.x);
        Velocity.x = dashForce * dashDirection;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }
    */
}
