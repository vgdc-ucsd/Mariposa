using System;
using Unity.VisualScripting;
using UnityEngine;

public class LaserProjectile : Projectile
{
    // this code should belong in a linerenderercontroller, but eh
    [SerializeField] private LineRenderer lineRenderer;
    private Transform[] points;

    // private ContactFilter2D playerAndBarrierFilter;
    private LayerMask playerAndBarrierMask;
    private LayerMask barrierMask;

    public override void Initialize(Vector3 position, Vector2 direction)
    {
        base.Initialize(position, direction);

        playerAndBarrierMask = LayerMask.GetMask("Player", "Barrier");
        barrierMask = LayerMask.GetMask("Barrier");

        // playerAndBarrierFilter.SetLayerMask(playerAndBarrierMask);
        // 57.3f = 180 / pi
        RaycastHit2D hitPlayerRay = Physics2D.BoxCast(this.transform.position, new(lineRenderer.startWidth / 2.0f, lineRenderer.startWidth / 2.0f),
        (float)Math.Atan2(direction.y, direction.x) * 57.3f, this.direction, 100.0f, playerAndBarrierMask);

        if (hitPlayerRay)
        {
            if (hitPlayerRay.collider.gameObject.CompareTag("Player"))
            {
                StartCoroutine(hitPlayerRay.collider.gameObject.GetComponent<Player>().Die());
            }
        }

        RaycastHit2D hitBarrierRay = Physics2D.BoxCast(this.transform.position, new(lineRenderer.startWidth / 2.0f, lineRenderer.startWidth / 2.0f),
        (float)Math.Atan2(direction.y, direction.x) * 57.3f, this.direction, 30.0f, barrierMask);

        Vector3[] endPoints = { this.transform.position, this.transform.position };
        if (hitBarrierRay)
        {
            endPoints[1] = (Vector3)hitBarrierRay.point;

        }
        else
        {
            endPoints[1] = this.transform.position + ((Vector3)direction * 30.0f);
        }
        SetPoints(endPoints);
    }

    public void SetPoints(Transform[] points)
    {
        lineRenderer.positionCount = points.Length;
        this.points = points;
    }

    public void SetPoints(Vector3[] points)
    {
        lineRenderer.positionCount = points.Length;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            // this.points[i].position = points[i];
            lineRenderer.SetPosition(i, points[i]);
        }
    }

    /// <summary>
    /// updates per frame while projectile trail is alive. edits color gradient  to fade over time
    /// </summary>
    /// <param name="dt">time between updates</param>
    public override void UpdateBehavior(float dt)
    {
        float deltaAlpha = -dt / lifetimeDuration;
        Debug.Log(deltaAlpha);
        Gradient gradient = new();
        gradient.SetKeys
        (
            new GradientColorKey[] { new(Color.white, 0.0f), new(Color.white, 1.0f) },
            new GradientAlphaKey[] { new(Math.Max(lineRenderer.colorGradient.alphaKeys[0].alpha + deltaAlpha, 0.0f), 0.0f), new(Math.Max(lineRenderer.colorGradient.alphaKeys[1].alpha + deltaAlpha, 0.0f), 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
    }
}
