using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public enum GrappleState
{
    Idle, Firing, Pulling, Stopped
}

public class GrappleAbility : MonoBehaviour, IAbility
{
    public static GrappleAbility Instance;

    [SerializeField]
    private GameObject hookProjectile;

    [SerializeField] private float hookSpeed = 50f;

    [SerializeField]
    private GrappleTarget currentTarget;
    private List<GrappleTarget> grappleTargets;


    private GrappleState state;
    private GrappleTarget lockedTarget;
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
    private Vector2 storedMomentum;
    [SerializeField]
    private float retentionDuration = 1f;   // how long the player still keeps the speed boost when jumping, even after stopping

    [SerializeField]
    private float retentionTimer = 0f;

    [SerializeField]
    private float grappleRange = 8f;

    private void Awake()
    {
        Instance = this;
        state = GrappleState.Idle;
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        grappleTargets = FindObjectsByType<GrappleTarget>(FindObjectsSortMode.None).ToList();
        hookProjectile.transform.SetParent(transform.parent);
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
            GrappleTowards(currentTarget);
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

    public void Initialize() { }

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
                if (HasLineOfSightToTarget(target))
                {
                    if (dir == targetDir)
                    {
                        potentialTargets.Add(target);
                    }
                    else behindTargets.Add(target);
                }
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

    private bool HasLineOfSightToTarget(GrappleTarget target)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Barrier"));
        RaycastHit2D[] hit = new RaycastHit2D[10];
        Physics2D.Raycast(Player.ActivePlayer.transform.position, target.transform.position - Player.ActivePlayer.transform.position, filter, hit);
        Debug.DrawLine(Player.ActivePlayer.transform.position, hit[0].point);
        if (hit[0].distance < Vector2.Distance(Player.ActivePlayer.transform.position, target.transform.position))
        {
            return false;
        }
        return true;
    }

    // fire the hook towards the target
    private void GrappleTowards(GrappleTarget target)
    {
        Player player = Player.ActivePlayer;
        lockedTarget = target;
        player.TurnTowards((int)Mathf.Sign(lockedTarget.transform.position.x - player.transform.position.x));
        player.Movement.Stop();
        player.Movement.ToggleGravity(false);
        ChangeGrappleState(GrappleState.Firing);
    }

    // hook is travelling towards the target
    private void GrappleFire()
    {
        hookProjectile.transform.position = Vector2.MoveTowards(hookProjectile.transform.position, currentTarget.transform.position, hookSpeed * fdt);
        if (Vector2.Distance(hookProjectile.transform.position, currentTarget.transform.position) < 1f)
        {
            ChangeGrappleState(GrappleState.Pulling);
        }

    }

    // Acceelrating towards the grapple point
    private void GrapplePull()
    {
        Vector2 playerPos = Player.ActivePlayer.transform.position;
        Player.ActivePlayer.Movement.Velocity += ((Vector2)lockedTarget.transform.position - playerPos) * grappleForce * fdt;
        if (Player.ActivePlayer.Movement.Velocity.magnitude > maxSpeed)
        {
            Player.ActivePlayer.Movement.Velocity = Player.ActivePlayer.Movement.Velocity.normalized * maxSpeed;
        }

        storedMomentum = Player.ActivePlayer.Movement.Velocity;

        if (Vector2.Distance(playerPos, (Vector2)lockedTarget.transform.position) < STOP_DISTANCE)
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
        Vector2 r = ((Vector2)lockedTarget.transform.position - playerPos);
        Player.ActivePlayer.Movement.Velocity += r * Mathf.Max(1, Mathf.Pow(r.magnitude, 2)) * closePullForce * fdt;
        if (retentionTimer > 0)
        {
            retentionTimer -= fdt;
        }
        else
        {
            storedMomentum = Vector2.zero;
        }

    }

    // Launch in a slightly upward arc with stored momentum
    private void GrappleJump()
    {
        if (!(state == GrappleState.Pulling || state == GrappleState.Stopped)) return;

        Vector2 launchDir = storedMomentum.normalized;
        Player.ActivePlayer.Movement.Velocity = launchDir * Mathf.Clamp(storedMomentum.magnitude + baseLaunchForce, baseLaunchForce, maxLaunchForce);

        GrappleRelease();

    }

    // Maintain the current speed
    private void GrappleRelease()
    {
        state = GrappleState.Idle;
        storedMomentum = Vector2.zero;
        Player.ActivePlayer.Movement.ToggleGravity(true);
        lockedTarget.ReleaseGrapple();
    }



    private void RenderLine()
    {
        if (new GrappleState[] { GrappleState.Pulling, GrappleState.Firing }.Contains(state))
        {
            Vector2 pos2;
            if (state == GrappleState.Firing)
            {
                pos2 = hookProjectile.transform.position;
            }
            else
            {
                pos2 = (Vector2)lockedTarget.transform.position;
            }
            lineRenderer.enabled = true;
            lineRenderer.SetPositions(new Vector3[]
            {
                gameObject.transform.position,
                pos2
            });
        }
        else
        {
            lineRenderer.enabled = false;
        }

    }

    private void PlayGrappleThrow()
    {
        EventInstance footstepInstance = RuntimeManager.CreateInstance("event:/sfx/player/grapple/impact");
        footstepInstance.start();
        footstepInstance.release();
    }

    private void ChangeGrappleState(GrappleState newState)
    {
        GrappleState previous = state;
        state = newState;

        if (previous != newState)
        {
            if (newState == GrappleState.Firing)
            {
                hookProjectile.SetActive(true);
                hookProjectile.transform.position = Player.ActivePlayer.transform.position;
                PlayGrappleThrow();
            }
            else
            {
                hookProjectile.SetActive(false);
            }
        }
    }

    public void RemoveGrappleTarget(GrappleTarget target)
    {
        grappleTargets.Remove(target);
    }
}