using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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
    public BoxCollider2D SurfaceCollider;
    protected List<RaycastHit2D> collisionHits = new(); // All collision hits for the current frame

    protected float fdt; // Shorthand for fixed delta time

    [Tooltip("The downward vertical acceleration applied when in the air")]
    public float Gravity;

    [Tooltip("The maximum fall velocity")]
    [SerializeField] protected float TerminalVelocity;

    // Layers that this object receives forces from
    [SerializeField] protected LayerMask collisionLayer;


    protected const float CONTACT_OFFSET = 0.005f; // The gap between this body and a surface after a collision
    protected const float COLLIDER_SHRINK_FACTOR = 0.995f; // The factor by which the collider is shrunk to make collision smoother

    protected const float LAND_SLOPE_FACTOR = 0.9f; // How horizontal a surface must be to be a ceiling or the ground
    protected const float COLLISION_CHECK_DISTANCE = 0.1f; // how far away you have to be from a ceiling or the ground to be considered "colliding" with it

    protected override void Awake()
    {
        base.Awake();
        Unlock();
        SurfaceCollider = GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        fdt = Time.deltaTime;

        Fall();
        CheckGrounded();

        Vector2 movement = Velocity * fdt;
        ApplyMovement(movement);
    }


    protected virtual void Fall()
    {
        if (State != BodyState.InAir) return;
        Velocity.y = Mathf.Max(Velocity.y - Gravity * fdt, -TerminalVelocity); // Cap the velocity 
    }

    protected virtual void CheckGrounded()
    {
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
        collisionHits.Clear();
        if (Mathf.Approximately(move.magnitude, 0f)) return; // This avoids weird imprecision errors

        Bounds bounds = SurfaceCollider.bounds;
        collisionHits.AddRange(Physics2D.BoxCastAll(bounds.center, bounds.size * COLLIDER_SHRINK_FACTOR, 0f, move.normalized, move.magnitude, collisionLayer));

        if (collisionHits.Count == 0) // no collisions
        {
            transform.position += (Vector3)move;
            return;
        }

        if (collisionHits.Count > 2) Debug.LogError("Can't handle more than 2 collisions at once");

        if (collisionHits.Count == 1) // single collision
        {
            RaycastHit2D hit = collisionHits[0];

            // Snap to the surface plus a small offset to avoid imprecision errors
            transform.position = collisionHits[0].centroid + collisionHits[0].normal.normalized * CONTACT_OFFSET;

            // Resolve the collision
            if (Mathf.Abs(hit.normal.y) > Mathf.Abs(hit.normal.x)) // Vertical collision
            {
                Velocity.y = 0;
                transform.position += (1 - hit.fraction) * move.x * Vector3.right;
            }
            else // Horizontal collision
            {
                Velocity.x = 0;
                transform.position += (1 - hit.fraction) * move.y * Vector3.up;
            }
        }
        else // double collision
        {
            if (Mathf.Abs(collisionHits[0].normal.y) > Mathf.Abs(collisionHits[0].normal.x)) // hit 0 was the vertical collision
            {
                transform.position = new Vector2(collisionHits[1].centroid.x + CONTACT_OFFSET * collisionHits[1].normal.normalized.x, 
                                                collisionHits[0].centroid.y + CONTACT_OFFSET * collisionHits[0].normal.normalized.y);
            }
            else // hit 1 was the vertical collision
            {
                transform.position = new Vector2(collisionHits[0].centroid.x + CONTACT_OFFSET * collisionHits[0].normal.normalized.x, 
                                                collisionHits[1].centroid.y + CONTACT_OFFSET * collisionHits[1].normal.normalized.y);
            }
            Velocity = Vector2.zero;
        }
    }

    // Drop players in the air at the start of a scene or after an interaction
    public virtual void Unlock()
    {
        State = BodyState.InAir;
    }
}