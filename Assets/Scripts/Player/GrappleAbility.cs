using UnityEngine;

public enum GrappleState
{
    Idle, Firing, Pulling, BoostRetention, Stopped
}

public class GrappleAbility : MonoBehaviour
{
    private GrappleState state;
    private Vector2 currentTarget;

    private const float MIN_DISTANCE = 2;
    private float grappleForce = 1;
    private float fdt;

    private void Awake()
    {
        state = GrappleState.Idle;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            GrappleTowards(Vector2.zero);
        }

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
        }
    }

    private void GrappleTowards(Vector2 target)
    {
        currentTarget = target;
        state = GrappleState.Firing;
    }

    private void GrappleFire()
    {
        state = GrappleState.Pulling;
    }

    private void GrapplePull()
    {
        Vector2 playerPos = Player.ActivePlayer.transform.position;
        Player.ActivePlayer.Movement.Velocity += (currentTarget - playerPos) * grappleForce * fdt;
    }
}