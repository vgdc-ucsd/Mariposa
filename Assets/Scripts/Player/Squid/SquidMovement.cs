using NUnit.Framework.Internal.Commands;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SquidMovement : FreeBody, IInputListener, IControllable
{
    public static SquidMovement Instance;
    private Bee parent;

    private Vector2 velocityFieldVelocity = Vector2.zero;

    [Header("Horizontal Parameters")]

    [Tooltip("The maximum horizontal movement speed")]
    public float MoveSpeed = 10;

    private Vector2 moveDir = Vector2.zero;

    [Tooltip("The time it takes to accelerate to maximum horizontal speed from rest in the air")]
    [SerializeField] private float airAccelerationTime;
    private float airAcceleration;

    [Tooltip("The time it takes to decelerate to rest by moving from maximum horizontal speed in the air")]
    [SerializeField] private float airMovementDecelerationTime;
    private float airMovementDeceleration;

    [Tooltip("The time it takes to decelerate to rest via drag from maximum horizontal speed in the air")]
    [SerializeField] private float airDragDecelerationTime;
    private float airDragDeceleration;

    public IBeeBehavior CurrentBehavior;    // Strategy, returns a vector for the bee to move towards in the next frame

    SpriteRenderer beeSprite;

    // Useful for when their dependent values are changed during runtime

    // only activate triggers when it is being controlled
    public override bool ActivateTriggers => parent.IsControlled;

    private void InitDerivedConsts()
    {
        airAcceleration = MoveSpeed / airAccelerationTime;
        airMovementDeceleration = MoveSpeed / airMovementDecelerationTime;
        airDragDeceleration = MoveSpeed / airDragDecelerationTime;
    }

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
        gravityEnabled = false;
        InitDerivedConsts();
        beeSprite = GetComponent<SpriteRenderer>();

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
    }

    public void SetMoveDir(Vector2 dir)
    {
        moveDir = dir;

        // change where the bee faces when in being controlled
        if (dir.x > 0) {
            beeSprite.flipX = false;
        }
        else if (dir.x < 0) {
            beeSprite.flipX = true;
        }
    }

    public void SetBehavior(IBeeBehavior behavior)
    {
        CurrentBehavior = behavior;
    }

    private void Move()
    {


        Vector2 controlDir = moveDir.normalized;
        bool neutral = Mathf.Approximately(controlDir.magnitude, 0);



        // Check which acceleration parameter to use
        float accelerationParam = (Vector2.Dot(controlDir, Velocity) > 0)
            ? airAcceleration
            : airMovementDeceleration;
        float acceleration = accelerationParam;

        Vector2 dv = acceleration * fdt * controlDir; // applied force


        // on no inputs, slow down by drag
        if (neutral) dv = -Velocity * airDragDeceleration * fdt;

        Vector2 tempVelocity = Velocity + dv;

        // if max speed is exceeded, slow down
        if (tempVelocity.magnitude > MoveSpeed)
        {
            tempVelocity *= MoveSpeed / tempVelocity.magnitude;
        }

        tempVelocity += velocityFieldVelocity;

        Velocity = tempVelocity;

        // remove all speed when close to at rest and not moving in any direction
        if (neutral && Velocity.magnitude <= 0.05f)
        {
            Velocity = Vector2.zero;
        }

    }

    public void ToggleCollisions(bool toggle)
    {
        collisionsEnabled = toggle;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("VelocityField"))
        {
            velocityFieldVelocity = collision.GetComponent<VelocityField>().velocity;
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        if (collision.CompareTag("VelocityField"))
        {
            velocityFieldVelocity = Vector2.zero;
        }
    }


}