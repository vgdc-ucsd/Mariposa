using NUnit.Framework.Internal.Commands;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class BeeMovement : FreeBody, IInputListener, IControllable
{
    public static PlayerMovement Instance;

    [Header("Horizontal Parameters")]

    [Tooltip("The maximum horizontal movement speed")]
    public float MoveSpeed = 10;

    private Vector2 horzMoveDir = Vector2.zero;
    private Vector2 vertMoveDir = Vector2.zero;

    [Tooltip("The time it takes to accelerate to maximum horizontal speed from rest in the air")]
    [SerializeField] private float airAccelerationTime;
    private float airAcceleration;

    [Tooltip("The time it takes to decelerate to rest by moving from maximum horizontal speed in the air")]
    [SerializeField] private float airMovementDecelerationTime;
    private float airMovementDeceleration;

    [Tooltip("The time it takes to decelerate to rest via drag from maximum horizontal speed in the air")]
    [SerializeField] private float airDragDecelerationTime;
    private float airDragDeceleration;


    // Useful for when their dependent values are changed during runtime
    private void InitDerivedConsts()
    {
        airAcceleration = MoveSpeed / airAccelerationTime;
        airMovementDeceleration = MoveSpeed / airMovementDecelerationTime;
        airDragDeceleration = MoveSpeed / airDragDecelerationTime;
    }

    protected override void Awake()
    {
        base.Awake();
        gravityEnabled = false;
        InitDerivedConsts();

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


    // public method to send a move command
    public void HorzMoveTowards(int dir)
    {
        // TurnTowards only changes "facing direction", which has no effect on movement
        // if (dir != 0) Player.ActivePlayer.TurnTowards(dir);

        if (dir == 1) horzMoveDir = Vector2.right;
        else if (dir == -1) horzMoveDir = Vector2.left;
        else horzMoveDir = Vector2.zero;
    }

    public void VertMoveTowards(int dir)
    {
        if (dir == 1) vertMoveDir = Vector2.up;
        else if (dir == -1) vertMoveDir = Vector2.down;
        else vertMoveDir = Vector2.zero;
    }

    private void Move()
    {
        Vector2 dir = (horzMoveDir + vertMoveDir).normalized;

        //if (dir != 0) Player.ActivePlayer.FacingDirection = dir;


        // Check which acceleration parameter to use
        float accelerationParam = (Vector2.Dot(dir,Velocity) > 0)
            ? airAcceleration
            : (dir.magnitude == 0)
                    ? airDragDeceleration
                    : airMovementDeceleration;

        float acceleration = accelerationParam;

        Vector2 tempVelocity = Velocity + acceleration * fdt * dir;

        // if max speed is exceeded, slow down
        if (tempVelocity.magnitude > MoveSpeed)
        {
            tempVelocity *= MoveSpeed / tempVelocity.magnitude;
        }
        Velocity = tempVelocity;
    }


}