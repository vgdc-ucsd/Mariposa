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
    public Collider2D Collider;
    public ContactPoint2D[] Contacts = new ContactPoint2D[20];
    public LayerMask LayerMask;


    private float dt;
    public float Gravity;

    private float GROUNDING_BOX_OFFSET = 0.05f;
    private const float LAND_SLOPE_FACTOR = 0.9f;

    protected virtual void Awake()
    {
        Unlock();
        Collider = GetComponent<Collider2D>();
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
            Velocity.y -= Gravity * dt;
        }
    }

    // Find all colliders touching with this body
    protected virtual void CheckCollisions()
    {
        Collider.GetContacts(Contacts);
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.NoFilter();
    }

    // If this body is in the air and has a barrier below it, land it
    // Otherwise, if this body is grounded and there is no barrier below it, start falling
    protected virtual void CheckGrounded()
    {
        foreach (ContactPoint2D contactPoint in Contacts)
        {
            // vertical collisions
            if (State == BodyState.InAir && contactPoint.normal.normalized.y > LAND_SLOPE_FACTOR)
            {
                Land(contactPoint);
            }
            // horizontal collisions
            // (todo)
        }

        if (State == BodyState.OnGround)
        {
            // while grounded, use a collider with a slight vertical offset downwards
            // to check if body is still grounded, to remove any edge cases causing jank

            Vector2 groundingBox = (Vector2)transform.position + Collider.offset + Vector2.down * GROUNDING_BOX_OFFSET;
            var groundingCollider = Physics2D.OverlapBox(groundingBox, Collider.bounds.size, 0f);
            ContactPoint2D[] groundContacts = new ContactPoint2D[20];
            groundingCollider.GetContacts(groundContacts);
            bool grounded = false;
            foreach (ContactPoint2D contactPoint in groundContacts)
            {
                // vertical collisions
                if (contactPoint.normal.normalized.y > LAND_SLOPE_FACTOR)
                {
                    grounded = true;
                }
                // horizontal collisions
                // (todo)
            }
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
            transform.position = new Vector2(transform.position.x, point.point.y + Collider.bounds.size.y / 2);
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