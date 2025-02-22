using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum GrappleState
{
    Idle, Firing, Pulling, Stopped
}

public class GrappleAbility : MonoBehaviour, IAbility
{
    [SerializeField]
    private GrappleTarget currentTarget;
    private List<GrappleTarget> grappleTargets;


    private GrappleState state;
    private Vector2 lockedTarget;
    private LineRenderer lineRenderer;

    // when is the player considered "close"
    private const float STOP_DISTANCE = 1;
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

    [SerializeField]
    private float grappleRange = 8f;

    private void Awake()
    {
        state = GrappleState.Idle;
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        grappleTargets = FindObjectsByType<GrappleTarget>(FindObjectsSortMode.None).ToList();
    }

    private void Update()
    {
        if (state == GrappleState.Idle)
        {
            FindGrapplePoint();
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

    public void AbilityInputDown()
    {
        if (state == GrappleState.Idle && currentTarget != null)
        {
            GrappleTowards(currentTarget.transform.position);
        }
    }

    public void AbilityInputUp()
    {
        if (state != GrappleState.Idle)
        {
            GrappleRelease();
        }
    }

    public void JumpInputDown()
    {
        if (state != GrappleState.Idle)
        {
            GrappleJump();
        }
    }

    private void FindGrapplePoint()
    {
        List<GrappleTarget> potentialTargets = new();
        List<GrappleTarget> behindTargets = new();
        int dir = Player.ActivePlayer.FacingDirection;

        foreach (GrappleTarget target in grappleTargets)
        {
            int targetDir = target.transform.position.x < Player.ActivePlayer.transform.position.x ? -1 : 1;
            float dist = Vector2.Distance(target.transform.position, Player.ActivePlayer.transform.position);
            if (dist <= grappleRange)
            {
                if (dir == targetDir)
                {
                    potentialTargets.Add(target);
                }
                else behindTargets.Add(target);
            }
        }

        // only if nothing in player's facing direction, check behind the player
        if (potentialTargets.Count == 0) potentialTargets = behindTargets;


        GrappleTarget closest = potentialTargets.OrderBy(target => Vector2.Distance(target.transform.position, Player.ActivePlayer.transform.position)).FirstOrDefault();

        if (closest == currentTarget) return;

        if (currentTarget != null) currentTarget.ToggleHighlight(false);
        currentTarget = closest;
        if (closest != null)
        {
            currentTarget.ToggleHighlight(true);
        }
        
    }

    // fire the hook towards the target
    private void GrappleTowards(Vector2 target)
    {
        Player player = Player.ActivePlayer;
        lockedTarget = target;
        player.TurnTowards((int)Mathf.Sign(target.x - player.transform.position.x));
        player.Movement.Stop();
        player.Movement.ToggleGravity(false);
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
        Player.ActivePlayer.Movement.Velocity += (lockedTarget - playerPos) * grappleForce * fdt;
        if (Player.ActivePlayer.Movement.Velocity.magnitude > maxSpeed)
        {
            Player.ActivePlayer.Movement.Velocity = Player.ActivePlayer.Movement.Velocity.normalized * maxSpeed;
        }

        storedMomentum = Mathf.Abs(Player.ActivePlayer.Movement.Velocity.x);

        if (Vector2.Distance(playerPos, lockedTarget) < STOP_DISTANCE)
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
        Vector2 r = (lockedTarget - playerPos);
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
        Player.ActivePlayer.Movement.Velocity = launchDir * Mathf.Clamp(storedMomentum + baseLaunchForce, baseLaunchForce, maxLaunchForce);

        GrappleRelease();

    }

    // Maintain the current speed
    private void GrappleRelease()
    {
        state = GrappleState.Idle;
        storedMomentum = 0;
        Player.ActivePlayer.Movement.ToggleGravity(true);
    }



    private void RenderLine()
    {
        if (new GrappleState[] { GrappleState.Pulling, GrappleState.Firing }.Contains(state))
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPositions(new Vector3[]
            {
                gameObject.transform.position,
                lockedTarget
            });
        }
        else
        {
            lineRenderer.enabled = false;
        }

    }


}