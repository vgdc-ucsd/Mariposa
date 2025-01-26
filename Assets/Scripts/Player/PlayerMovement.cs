using NUnit.Framework.Internal.Commands;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    private Rigidbody2D rb;
    private Collider2D col;
    [SerializeField] private LayerMask playerLayer;

    public float MoveSpeed = 10;
    [SerializeField] private float TerminalVelocity; // the maximum fall velocity
    public float Gravity;

    private const float COLLISION_CHECK_DISTANCE = 0.05f; // how far away you have to be from a surface to be considered "colliding" with it
    private const float LAND_SLOPE_FACTOR = 0.9f; // magnitude of the y component of a surface normal to be considered vertical

    public BodyState State;

    private Vector2 velocity; // the velocity for the current frame
    private float fdt; // Shorthand for fixed delta time

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        State = BodyState.InAir; // Initialize the player as in the air

        Instance = this;
    }

    private void FixedUpdate()
    {
        fdt = Time.fixedDeltaTime;

        if (Input.GetKey(KeyCode.D)) Move(1);
        else if (Input.GetKey(KeyCode.A)) Move(-1);
        else Move(0);

        if (Input.GetKey(KeyCode.Space)) Jump();

        Fall();
        CheckGrounded();
        CheckCeilingHit();
        ApplyFriction();

        UpdateVelocity();
    }

    // Move by player input: 1 = right, -1 = left
    private void Move(int dir)
    {
        velocity.x = MoveSpeed * dir;
    }

    // Directly set the player's y velocity
    private void Jump()
    {
        velocity.y = MoveSpeed; // this should be its own value
    }

    // Apply gravity in the air
    private void Fall()
    {
        if (State != BodyState.InAir) return;

        velocity.y = Mathf.Max(velocity.y - Gravity * fdt, -TerminalVelocity);
    }

    // Check if the player is about to collide with the ground and update the player's bodyState accordingly
    private void CheckGrounded()
    {
        RaycastHit2D groundHit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, COLLISION_CHECK_DISTANCE, ~playerLayer);

        if (State != BodyState.OnGround && groundHit && groundHit.normal.normalized.y > LAND_SLOPE_FACTOR)
        {
            State = BodyState.OnGround;
            velocity.y = 0;
        }
        else if (State == BodyState.OnGround && !groundHit)
        {
            State = BodyState.InAir;
        }
    }

    // Make the player fall immediately after hitting a ceiling
    private void CheckCeilingHit()
    {
        // Similar raycast to CheckGrounded
        RaycastHit2D ceilingHit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.up, COLLISION_CHECK_DISTANCE, ~playerLayer);

        if (ceilingHit && -ceilingHit.normal.normalized.y > LAND_SLOPE_FACTOR) velocity.y = Mathf.Min(0, velocity.y);
    }

    private void ApplyFriction()
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

    // Apply any changes to the velocity to the actual rigidbody velocity
    private void UpdateVelocity() => rb.linearVelocity = velocity;
}
