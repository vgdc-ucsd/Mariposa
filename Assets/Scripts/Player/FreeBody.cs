using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

// The external forces this object can currently experience
public enum BodyState
{
    OnGround, // Experiences sliding friction
    InAir, // Experiences gravity, (horizontal) air friction, and terminal drag
    Locked  // Cannot move and does not experience any force
}



// Any solid object that moves and falls freely, but interacts with barriers (e.g. floor).
// Common behavior is defined in the abstract class (e.g. falling, sliding on ground),
// and each individual class defines any additional movements
public abstract class FreeBody : Body
{
    public BodyState State;
    
    protected List<RaycastHit2D> collisionHits = new(); // All collision hits for the current frame

    protected float fdt; // Shorthand for fixed delta time

    [Tooltip("The downward vertical acceleration applied when in the air")]
    public float Gravity;

    protected bool gravityEnabled = true;
    protected bool collisionsEnabled = true;

    [Tooltip("The maximum fall velocity")]
    public float TerminalVelocity;

    [Tooltip("The minimum angle in degrees a slope must make to the ground to cause slipping")]
    [SerializeField] protected float slipAngle = 45f;

    // Layers that this object receives forces from
    [SerializeField] protected LayerMask collisionLayer;


    public const float CONTACT_OFFSET = 0.005f; // The gap between this body and a surface after a collision

    protected const float LAND_SLOPE_FACTOR = 0.9f; // How horizontal a surface must be to be a ceiling or the ground
    protected const float COLLISION_CHECK_DISTANCE = 0.1f; // how far away you have to be from a ceiling or the ground to be considered "colliding" with it
    private const float MAX_SUBSTEPS = 5; // The maximum number of movement substeps ApplyMovement can take;

    protected override void Awake()
    {
        base.Awake();
        Unlock();
        

        ResolveInitialCollisions();
    }

    public void ResolveInitialCollisions()
    {
        Physics2D.SyncTransforms();
        Collider2D[] initialContacts = Physics2D.OverlapBoxAll(SurfaceCollider.bounds.center, SurfaceCollider.bounds.size, 0, collisionLayer);
        foreach (Collider2D collider in initialContacts)
        {
            ColliderDistance2D separation = SurfaceCollider.Distance(collider);
            transform.position += (separation.distance + CONTACT_OFFSET) * (Vector3)separation.normal;
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    private Vector2 moveVec = Vector2.zero;

    protected override void FixedUpdate()
    {
        fdt = Time.deltaTime;

        CheckGrounded();
        Fall();

        Vector2 movement = Velocity * fdt;
        moveVec = movement;
        ApplyMovement(movement);
    }


    protected virtual void Fall()
    {
        if (State != BodyState.InAir || !gravityEnabled) return;
        Velocity.y = Mathf.Max(Velocity.y - Gravity * fdt, -TerminalVelocity); // Cap the velocity
    }

    protected virtual void CheckGrounded()
    {
        if (!collisionsEnabled) return;

        Bounds bounds = SurfaceCollider.bounds;
        Vector2 origin = (Vector2)transform.position + SurfaceCollider.offset + SurfaceCollider.bounds.extents.y * 3/4 * Vector2.down;
        Vector2 size = new(bounds.size.x * 0.99f, bounds.size.y / 4);
        Physics2D.SyncTransforms();
        RaycastHit2D groundHit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, Mathf.Infinity, collisionLayer);

        bool didHitGround = groundHit && groundHit.distance <= COLLISION_CHECK_DISTANCE;
        if (groundHit && !didHitGround && groundHit.collider.CompareTag("MovingPlatform"))
        {
            MovingPlatform movingPlatform = groundHit.collider.GetComponentInParent<MovingPlatform>();
            didHitGround = movingPlatform.currMovement.y < 0f && groundHit.distance <= COLLISION_CHECK_DISTANCE - movingPlatform.currMovement.y;
        }

        bool isSlipSlope = groundHit && Mathf.Abs(Mathf.Atan(groundHit.normal.y / groundHit.normal.x) * Mathf.Rad2Deg) <= slipAngle;

        if (didHitGround && !isSlipSlope) OnGrounded(groundHit);
        else if (!didHitGround || isSlipSlope) OnAirborne();
    }

    protected virtual void OnGrounded(RaycastHit2D groundHit)
    {
        if (State != BodyState.OnGround) State = BodyState.OnGround;
    }

    // 
    protected virtual void OnAirborne()
    {
        if (State == BodyState.OnGround) State = BodyState.InAir;
    }

    /// <summary>
    /// Applies a movement vector to this freebody and respects collision
    /// </summary>
    /// <param name="move">The movement vector</param>
    public virtual void ApplyMovement(Vector2 move)
    {
        if (!collisionsEnabled) return;
        collisionHits.Clear();
        if (Mathf.Approximately(move.magnitude, 0f)) return; // This avoids weird imprecision errors

        Bounds bounds = SurfaceCollider.bounds;
        Vector2 origin = (Vector2)transform.position + SurfaceCollider.offset;
        RaycastHit2D hit = Physics2D.BoxCast(origin, bounds.size, 0f, move.normalized, move.magnitude, collisionLayer);

        // If the free body is inside another object, separate them and recompute the raycast
        if (hit && Mathf.Approximately(hit.distance, 0f))
        {
            ResolveInitialCollisions();
            origin = (Vector2)transform.position + SurfaceCollider.offset;
            hit = Physics2D.BoxCast(origin, bounds.size, 0f, move.normalized, move.magnitude, collisionLayer);
        }

        int substeps = 0; // This is to prevent infinite loops in case something goes wrong
        while (hit && !hit.collider.isTrigger)
        {
            collisionHits.Add(hit);
            Vector2 normal = hit.normal.normalized;
            Vector2 delta = hit.centroid - origin + CONTACT_OFFSET * normal;
            move -= delta;

            transform.position += (Vector3)delta;
            origin = (Vector2)transform.position + SurfaceCollider.offset;
            move -= Helper.Vec2Proj(move, normal);
            Velocity -= Helper.Vec2Proj(Velocity, normal);
            // Prevent slowly slipping when on a walkable slope
            //if (gravityEnabled && State == BodyState.OnGround) Velocity.y = Mathf.Max(Velocity.y, 0f);

            // These conditionals sidestep floating point imprecisions
            if (Mathf.Approximately(Velocity.sqrMagnitude, 0f)) 
            { 
                Velocity = Vector2.zero;
                break; 
            }
            if (Mathf.Approximately(move.sqrMagnitude, 0f))
            {
                move = Vector2.zero;
                break;
            }

            hit = Physics2D.BoxCast(origin, bounds.size, 0f, move.normalized, move.magnitude, collisionLayer);

            substeps++;
            if (substeps > MAX_SUBSTEPS) break;
        }

        // Apply lingering movement
        transform.position += (Vector3)move;
    }

    // Drop players in the air at the start of a scene or after an interaction
    public virtual void Unlock()
    {
        State = BodyState.InAir;
    }

    public void ToggleGravity(bool toggle)
    {
        gravityEnabled = toggle;
    }

    public void Stop()
    {
        Velocity = Vector3.zero;
    }
}