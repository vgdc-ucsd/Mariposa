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
    private EventInstance swingSoundInstance;

    // when is the player considered "close"
    private const float STOP_DISTANCE = 1;
    private const float MAX_GRAPPLE_FORCE = 65f;
    private const float BASE_GRAPPLE_FORCE = 50f;
    private const float GRAPPLE_FORCE_INCREASE = 0.15f;
    public float grappleForce = BASE_GRAPPLE_FORCE;
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
        UpdateGrappleTargets();
        hookProjectile.transform.SetParent(transform.parent);
    }

    private void OnEnable()
    {
        Player.OnDeath += ResetGrapple;
    }

    private void OnDisable()
    {
        Player.OnDeath -= ResetGrapple;
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
            if (!target.isAvailable) continue;

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

        bool canAutoGrapple = true;
        if (currentTarget != null)
        {
            canAutoGrapple = false;
            currentTarget.ToggleHighlight(false);
        }
        currentTarget = closest;
        if (closest != null)
        {
            currentTarget.ToggleHighlight(true);

            // Buffered grapple
            if (canAutoGrapple && PlayerController.Instance.CheckAbilityBuffer())
            {
                AbilityInputDown();
            }
        }

    }

    private RaycastHit2D[] lineOfSightCastHits = new RaycastHit2D[10];
    private bool HasLineOfSightToTarget(GrappleTarget target)
    {
        ContactFilter2D filter = new();
        filter.SetLayerMask(LayerMask.GetMask("Barrier"));
        Vector2 playerToTarget = target.transform.position - Player.ActivePlayer.transform.position;
        Physics2D.Raycast(Player.ActivePlayer.transform.position, playerToTarget, filter, lineOfSightCastHits);
        Debug.DrawLine(Player.ActivePlayer.transform.position, lineOfSightCastHits[0].point);
        return lineOfSightCastHits[0].collider == null
                || lineOfSightCastHits[0].distance >= playerToTarget.magnitude;
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
        RuntimeManager.PlayOneShot(AudioEvents.SFX.unnamed_grapple_throw);
    }

    // hook is travelling towards the target
    private void GrappleFire()
    {
        Vector2 direction = (currentTarget.transform.position - hookProjectile.transform.position).normalized;
        hookProjectile.transform.position = Vector2.MoveTowards(hookProjectile.transform.position, currentTarget.transform.position, hookSpeed * fdt);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        hookProjectile.transform.rotation = Quaternion.Euler(0, 0, angle);
        if (Vector2.Distance(hookProjectile.transform.position, currentTarget.transform.position) < 1f)
        {
            ChangeGrappleState(GrappleState.Pulling);

            RuntimeManager.PlayOneShot(AudioEvents.SFX.unnamed_grapple_impact);
            swingSoundInstance = AudioManager.CreateEventInstance(AudioEvents.SFX.unnamed_grapple_swing);
            if (swingSoundInstance.isValid()) swingSoundInstance.start();
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

        grappleForce = Mathf.Min(grappleForce + GRAPPLE_FORCE_INCREASE, MAX_GRAPPLE_FORCE);
        storedMomentum = Player.ActivePlayer.Movement.Velocity;

        if (Vector2.Distance(playerPos, (Vector2)lockedTarget.transform.position) < STOP_DISTANCE)
        {
            AbilityInputUp();
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
        if (state != GrappleState.Idle)
        {
            state = GrappleState.Idle;
            storedMomentum = Vector2.zero;
            grappleForce = BASE_GRAPPLE_FORCE;
            Player.ActivePlayer.Movement.ToggleGravity(true);
            hookProjectile.SetActive(false);
            lockedTarget.ReleaseGrapple();
            AudioManager.StopEventInstance(swingSoundInstance, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            swingSoundInstance = default;
        }
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
    public void UpdateGrappleTargets()
    {
        grappleTargets = FindObjectsByType<GrappleTarget>(FindObjectsSortMode.None).ToList();
    }

    private void ResetGrapple()
    {
        if (currentTarget != null)
        {
            GrappleRelease();
            currentTarget.ResetGrappleTarget();
        }
    }
}
