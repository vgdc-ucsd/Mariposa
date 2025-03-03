using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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
    [SerializeField] protected float TerminalVelocity;

    // Layers that this object receives forces from
    [SerializeField] protected LayerMask collisionLayer;


    protected const float CONTACT_OFFSET = 0.005f; // The gap between this body and a surface after a collision

    protected const float LAND_SLOPE_FACTOR = 0.9f; // How horizontal a surface must be to be a ceiling or the ground
    protected const float COLLISION_CHECK_DISTANCE = 0.1f; // how far away you have to be from a ceiling or the ground to be considered "colliding" with it

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

    protected override void FixedUpdate()
    {
        fdt = Time.deltaTime;

        CheckGrounded();
        Fall();

        Vector2 movement = Velocity * fdt;
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
        RaycastHit2D groundHit = Physics2D.BoxCast(bounds.center, bounds.size, 0f, Vector2.down, COLLISION_CHECK_DISTANCE, collisionLayer);
        if (State != BodyState.OnGround && groundHit && groundHit.normal.normalized.y > LAND_SLOPE_FACTOR)
        {
            State = BodyState.OnGround;
        }
        else if (State == BodyState.OnGround && !groundHit)
        {
            State = BodyState.InAir;
        }
    }

    protected virtual void ApplyMovement(Vector2 move)
    {
        if (!collisionsEnabled) return;
        collisionHits.Clear();
        if (Mathf.Approximately(move.magnitude, 0f)) return; // This avoids weird imprecision errors

        Bounds bounds = SurfaceCollider.bounds;
        RaycastHit2D hit = Physics2D.BoxCast(bounds.center, bounds.size, 0f, move.normalized, move.magnitude, collisionLayer);

        int loops = 0; // This is to prevent infinite loops in case something goes wrong
        while (hit && !(Mathf.Approximately(move.x, 0f) && Mathf.Approximately(move.y, 0f)))
        {
            Vector2 normal = hit.normal.normalized;
            collisionHits.Add(hit);

            if (Mathf.Abs(normal.y) > Mathf.Abs(normal.x)) // Vertical collision
            {
                float deltaY = Mathf.Abs(hit.centroid.y - bounds.center.y);
                transform.position += (deltaY * move.normalized.y + normal.y * CONTACT_OFFSET) * Vector3.up;
                move.y = 0;
                Velocity.y = 0;
            }
            else // Horizontal collision
            {
                float deltaX = Mathf.Abs(hit.centroid.x - bounds.center.x);
                transform.position += (deltaX * move.normalized.x + normal.x * CONTACT_OFFSET) * Vector3.right;
                move.x = 0;
                Velocity.x = 0;
            }
            hit = Physics2D.BoxCast(bounds.center, bounds.size, 0f, move.normalized, move.magnitude, collisionLayer);
            if (Mathf.Approximately(hit.distance, 0f))
            {
                ResolveInitialCollisions();
                break;
            }

            loops++;
            if (loops > 4)
            {
                break;
            }
        }


        // Apply the movement
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