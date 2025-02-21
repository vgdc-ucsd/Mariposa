using NUnit.Framework;
using System.Linq;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public enum GrappleState
{
    Idle, Firing, Pulling, Stopped
}

public class GrappleAbility : MonoBehaviour
{
    [SerializeField]
    private Transform testGrappleTarget;

    private GrappleState state;
    private Vector2 currentTarget;
    private LineRenderer lineRenderer;

    // when is the player considered "close"
    private const float STOP_DISTANCE = 3;
    public float grappleForce = 50;
    private float fdt;

    public float maxSpeed = 50f;

    // how quickly the player slows down upon coming close to the grapple point
    public float reachedTargetDamping = 5f;
    // how strongly the player is pulled towards the grapple point when coming close (scales like r^2 when r < 1)
    public float closePullForce = 5f;

    [SerializeField]
    private float baseLaunchForce = 10f;
    [SerializeField]
    private float maxLaunchForce = 30f;

    [SerializeField]
    private float storedMomentum;
    [SerializeField]
    private float retentionDuration = 1f;   // how long the player still keeps the speed boost when jumping, even after stopping

    [SerializeField]
    private float retentionTimer = 0f;


    private void Awake()
    {
        state = GrappleState.Idle;
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (state == GrappleState.Idle)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                GrappleTowards(testGrappleTarget.position);
            }
        }
        else
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                GrappleRelease();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                GrappleJump();
            }
        }

        RenderLine();
    }

    private void FixedUpdate()
    {
        fdt = Time.fixedDeltaTime;
        switch (state)
        {
            case GrappleState.Firing:
                GrappleFire();
                break;
            case GrappleState.Pulling:
                GrapplePull();
                break;
            case GrappleState.Stopped:
                GrappleCling();
                break;
        }
    }

    // fire the hook towards the target
    private void GrappleTowards(Vector2 target)
    {
        Player player = Player.ActivePlayer;
        currentTarget = target;
        player.TurnTowards((int)Mathf.Sign(target.x - player.transform.position.x));
        state = GrappleState.Firing;
    }

    // hook is travelling towards the target
    private void GrappleFire()
    {
        // todo line firing animation
        state = GrappleState.Pulling;
        
    }
    
    // Acceelrating towards the grapple point
    private void GrapplePull()
    {
        Vector2 playerPos = Player.ActivePlayer.transform.position;
        Player.ActivePlayer.Movement.Velocity += (currentTarget - playerPos) * grappleForce * fdt;
        if (Player.ActivePlayer.Movement.Velocity.magnitude > maxSpeed)
        {
            Player.ActivePlayer.Movement.Velocity = Player.ActivePlayer.Movement.Velocity.normalized * maxSpeed;
        }

        storedMomentum = Mathf.Abs(Player.ActivePlayer.Movement.Velocity.x);

        if (Vector2.Distance(playerPos, currentTarget) < STOP_DISTANCE)
        {
            state = GrappleState.Stopped;
            retentionTimer = retentionDuration;
        }
    }

    // In proximity of the grapple point, start slowing down
    private void GrappleCling()
    {
        Vector2 playerPos = Player.ActivePlayer.transform.position;
        Player.ActivePlayer.Movement.Velocity -= Player.ActivePlayer.Movement.Velocity * reachedTargetDamping * fdt;
        Vector2 r = (currentTarget - playerPos);
        Player.ActivePlayer.Movement.Velocity += r * Mathf.Max(1, Mathf.Pow(r.magnitude, 2)) * closePullForce * fdt;
        if (retentionTimer > 0)
        {
            retentionTimer -= fdt;
        }
        else
        {
            storedMomentum = 0;
        }
        
    }

    // Launch in a slightly upward arc with stored momentum
    private void GrappleJump()
    {
        if (!(state == GrappleState.Pulling || state == GrappleState.Stopped)) return;

        Vector2 launchDir = new Vector2(Player.ActivePlayer.FacingDirection, 0.4f).normalized;
        Player.ActivePlayer.Movement.Velocity = launchDir * Mathf.Clamp(storedMomentum, baseLaunchForce, maxLaunchForce);

        state = GrappleState.Idle;
        storedMomentum = 0;

    }

    // Maintain the current speed
    private void GrappleRelease()
    {
        state = GrappleState.Idle;
    }

    private void RenderLine()
    {
        if (new GrappleState[] { GrappleState.Pulling, GrappleState.Firing }.Contains(state))
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPositions(new Vector3[]
            {
                gameObject.transform.position,
                currentTarget
            });
        }
        else
        {
            lineRenderer.enabled = false;
        }

    }
}