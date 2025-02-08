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
    public List<ContactPoint2D> Contacts = new();

    protected float fdt; // Shorthand for fixed delta time

    [Tooltip("The downward vertical acceleration applied when in the air")]
    public float Gravity;
    [Tooltip("The maximum fall velocity")]
    [SerializeField] protected float TerminalVelocity;

    // Layers that this object receives forces from
    [SerializeField] private LayerMask collisionLayer;

    private const float COLLISION_CHECK_DISTANCE = 0.1f; // how far away you have to be from a surface to be considered "colliding" with it

    // collision boxcasts have widths smaller than the player hitbox by this amount on both sides
    private const float COLLISION_SURFACE_SPACING_VERT = 0.05f; // for vertical boxcasts
    private const float COLLISION_SURFACE_SPACING_HORZ = 0.15f; // for horizontal boxcasts, increase for more vertical corner correction

    private const float COLLISION_BOXCAST_SIZE = 0.1f;

    // how far away you have to be from a surface to be considered "collidiing" with it,
    // snapping towards the surface afterwards
    private const float LAND_SLOPE_FACTOR = 0.9f;


    public bool touchingLeft, touchingRight;

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
        CheckTouchingWalls();
        CheckTouchingCeiling();
        ApplyFriction();

        base.FixedUpdate();
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
            if (Velocity.y > -TerminalVelocity) {
                Velocity.y -= Gravity * fdt;
            }
            else {
                Velocity.y = -TerminalVelocity;
            }
        }
    }

    // Projects a boxcast from the edge of the player towards dir for a fixed distance
    // Retruns raycast results
    // dir must be an orthogonal basis vector (Vector2.up, down, left, right)
    private RaycastHit2D CollisionBoxCast(Vector2 dir)
    {
        float surfaceSize, normalSize;
        Vector2 boxSize;
        if (dir.x == 0)
        {
            // vertical

            // box doesn't cover player width fully for corner correction
            surfaceSize = SurfaceCollider.bounds.size.x - COLLISION_SURFACE_SPACING_VERT * 2;
            // height of the player
            normalSize = SurfaceCollider.bounds.size.y;
            // boxcast envelops bottom half of the player
            boxSize = new Vector2(surfaceSize, COLLISION_BOXCAST_SIZE);
        }
        else
        {
            // horizontal
            surfaceSize = SurfaceCollider.bounds.size.y - COLLISION_SURFACE_SPACING_HORZ * 2;
            normalSize = SurfaceCollider.bounds.size.x;
            boxSize = new Vector2(COLLISION_BOXCAST_SIZE, surfaceSize);
        }

        // boxcast edge starts from center of the player
        Vector2 boxCastStartPos = (Vector2)SurfaceCollider.bounds.center 
            + dir * (COLLISION_BOXCAST_SIZE / 2);
        float distance = normalSize / 2 - COLLISION_BOXCAST_SIZE / 2 + COLLISION_CHECK_DISTANCE;


        RaycastHit2D groundHit = Physics2D.BoxCast(
            boxCastStartPos,
            boxSize,
            0f, dir,
            distance,
            collisionLayer);
        return groundHit;
    }

    // Snaps the player to toubhing a surface when they come close to it
    // checkVelocity: whether snapping occurs only if player is moving towards the surface
    // returns whether snap actually occurred
    protected virtual bool SnapToSurface(RaycastHit2D hit, bool checkVelocity = false)
    {
        // reject if player is moving away from the surface
        if (checkVelocity && Vector2.Dot(hit.normal, Velocity) >= -0.0001f) return false;

        float normalSize = Mathf.Abs(Vector2.Dot(SurfaceCollider.bounds.size, hit.normal));

        // Calculate the correction based on the separation and normal
        Vector2 correction = hit.normal * -(hit.distance - normalSize / 2 + COLLISION_BOXCAST_SIZE);

        // Apply the correction and snap to surface
        transform.position += (Vector3)correction;
        return true;
    }

    // If this body is in the air and has a barrier below it, land it
    // Otherwise, if this body is grounded and there is no barrier below it, start falling
    protected virtual void CheckGrounded()
    {
        var groundHit = CollisionBoxCast(Vector2.down);
        if (groundHit)
        {
            if (groundHit.normal.normalized.y > LAND_SLOPE_FACTOR)
            {
                if (State == BodyState.InAir) Land(groundHit);
            }
        }
        else
        {
            if (State == BodyState.OnGround)
            {
                StartFalling();
            }
        }
        
    }

    // returns: whether there was an actual state change
    protected virtual bool StartFalling()
    {
        if (State == BodyState.OnGround)
        {
            State = BodyState.InAir;
            return true;
        }
        return false;
    }

    // returns: whether there was an actual state change
    protected virtual bool Land(RaycastHit2D hit)
    {
        if (State == BodyState.InAir)
        {
            if (SnapToSurface(hit, true))
            {
                State = BodyState.OnGround;
                Velocity.y = 0;
                return true;
            }
        }
        return false;
    }



    protected virtual void CheckTouchingWalls()
    {
        var leftCast = CollisionBoxCast(Vector2.left);
        var rightCast = CollisionBoxCast(Vector2.right);
        touchingLeft = leftCast;
        touchingRight = rightCast;
        if (leftCast)
        {
            TouchWall(leftCast, -1);
        }
        if (rightCast) TouchWall(rightCast, 1);
    }


    // dir -1 = left, 1 = right
    protected virtual void TouchWall(RaycastHit2D hit, int dir)
    {
        // don't reset velocity if moving away from wall
        if (!SnapToSurface(hit, true)) return;
        if (dir == -1) Velocity.x = Mathf.Max(0, Velocity.x);
        else if (dir == 1) Velocity.x = Mathf.Min(0, Velocity.x);
    }

    protected virtual RaycastHit2D CheckTouchingCeiling()
    {
        var ceilCast = CollisionBoxCast(Vector2.up);
        if (!ceilCast) return ceilCast;
        if (SnapToSurface(ceilCast, true))
        {
            Velocity.y = Mathf.Min(0, Velocity.y);
        }
        return ceilCast;
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