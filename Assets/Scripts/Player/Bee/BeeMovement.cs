using NUnit.Framework.Internal.Commands;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class BeeMovement : FreeBody, IInputListener, IControllable
{
    public static PlayerMovement Instance;
    private Bee parent;

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


    private const float RADIUS_THRESHOLD = 0.1f;  // distance to the max control radius that the bee is absolutely not allowed to pass
    private const float RADIUS_SLOWDOWN_BOUNDARY = 1f;  // how close to the max control radius to start slowing down
    private const float RESISTANCE_ADJUSTMENT = 0.3f;   // strength of the slowdown effect

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
        parent = GetComponent<Bee>();
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
        Vector2 r = transform.position - Player.ActivePlayer.transform.position;
        float distanceToPlayer = r.magnitude;

        if (distanceToPlayer > parent.MaxControlRadius) return;


        Vector2 controlDir = (horzMoveDir + vertMoveDir).normalized;
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


        Vector2 normalComp = Vector2.Dot(tempVelocity, r) / r.sqrMagnitude * r;


        Vector2 futurePos = (Vector2)transform.position + tempVelocity * fdt;
        float adjustedControlRadius = parent.MaxControlRadius - RADIUS_THRESHOLD;
        // if moving more will cause the bee to leave the control radius, force it to go back inside
        if (Vector2.Distance(futurePos, Player.ActivePlayer.transform.position) > adjustedControlRadius)
        {
            // remove the component that would lead to crossing the boundary
            float exceedsBy = Vector2.Distance(futurePos, Player.ActivePlayer.transform.position) - adjustedControlRadius;
            float currDistToBorder = adjustedControlRadius - distanceToPlayer;

            Vector2 exceedingComp = normalComp * exceedsBy / Mathf.Max((exceedsBy + currDistToBorder), 0.0001f);    // prevent div by 0
            tempVelocity -= exceedingComp;
        }

       
        // if moving more will cause the bee to come close to control radius, increase resistance in the normal direction
        else if (distanceToPlayer > adjustedControlRadius - RADIUS_SLOWDOWN_BOUNDARY)
        {
            float border = adjustedControlRadius - RADIUS_SLOWDOWN_BOUNDARY;
            // only count the normal component if it is pointing outwards
            Vector2 clampedNormalComp = normalComp * Mathf.Max(0, Vector2.Dot(normalComp.normalized, r.normalized));

            float resistance = Mathf.Clamp01((distanceToPlayer - border) / (adjustedControlRadius - border));
            Debug.Log(resistance);
            tempVelocity = tempVelocity - clampedNormalComp * resistance * RESISTANCE_ADJUSTMENT;
        }
        
        
        



        Velocity = tempVelocity;

        // remove all speed when close to at rest and not moving in any direction
        if (neutral && Velocity.magnitude <= 0.05f)
        {
            Velocity = Vector2.zero;
        }
    }


}