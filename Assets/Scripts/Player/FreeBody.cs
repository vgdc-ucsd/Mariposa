using System.Collections.Generic;
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
    public Bounds ActualColliderBounds;
    public List<ContactPoint2D> Contacts = new();

    private float dt;
    public float Gravity;

    private float COLLISION_SNAP_OFFSET = 0.05f;
    // how far away you have to be from a surface to be considered "collidiing" with it,
    // snapping towards the surface afterwards
    private const float LAND_SLOPE_FACTOR = 0.9f;
    const float TerminalVelocity = 40; 

    protected override void Awake()
    {
        base.Awake();
        Unlock();
        SurfaceCollider = GetComponent<BoxCollider2D>();
        ActualColliderBounds = SurfaceCollider.bounds;  // Save original collider size
        SurfaceCollider.size += Vector2.one * COLLISION_SNAP_OFFSET * 2;    // Add an offset to collider size, for reliable collision detection
    }

    protected override void Update()
    {
        dt = Time.deltaTime;

        Fall();
        CheckCollisions();
        CheckGrounded();

        ApplyFriction();

        base.Update();
    }

    // Drop players in the air at the start of a scene or after an interaction
    public virtual void Unlock()
    {
        State = BodyState.InAir;
    }

    // Apply gravitational acceleration in the air
    protected virtual void Fall()
    {
        if (State == BodyState.InAir)
        {
            if (Velocity.y < TerminalVelocity) {
                Velocity.y -= Gravity * dt;
            }
            else {
                Velocity.y = TerminalVelocity;
            }
        }
    }

    // Find all colliders touching with this body
    protected virtual void CheckCollisions()
    {
        
        SurfaceCollider.GetContacts(Contacts);
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.NoFilter();
    }

    // If this body is in the air and has a barrier below it, land it
    // Otherwise, if this body is grounded and there is no barrier below it, start falling
    protected virtual void CheckGrounded()
    {
        bool grounded = false;
        foreach (ContactPoint2D contactPoint in Contacts)
        {
            // vertical collisions
            if (contactPoint.normal.normalized.y > LAND_SLOPE_FACTOR)
            {
                grounded = true;
                if (State == BodyState.InAir) Land(contactPoint);
            }
        }
        if (State == BodyState.OnGround)
        {
            if (!grounded)
            {
                StartFalling();
            }
        }

        
    }

    protected virtual void StartFalling()
    {
        if (State == BodyState.OnGround)
        {
            State = BodyState.InAir;
        }
    }
    protected virtual void Land(ContactPoint2D point)
    {
        if (State == BodyState.InAir)
        {
            State = BodyState.OnGround;
            Velocity.y = 0;

            // Calculate the correction based on the separation and normal
            Vector2 correction = point.normal * -point.separation;


            // Apply the correction and snap to the ground
            transform.position += (Vector3)correction - Vector3.up * (COLLISION_SNAP_OFFSET + 2 * Physics2D.defaultContactOffset);

        }
    }



    protected virtual void ApplyFriction()
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

}