using System;
using UnityEngine;

public class LaserProjectile : Projectile
{
    // this code should belong in a linerenderercontroller, but eh
    [SerializeField] private LineRenderer lineRenderer;
    private Transform[] points;

    // private ContactFilter2D playerAndBarrierFilter;
    private LayerMask playerAndBarrierMask;

    public override void Initialize(Vector3 position, Vector2 direction)
    {
        base.Initialize(position, direction);

        playerAndBarrierMask = LayerMask.GetMask("Player", "Barrier");
        // playerAndBarrierFilter.SetLayerMask(playerAndBarrierMask);
        // 57.3f = 180 / pi
        RaycastHit2D boxCast = Physics2D.BoxCast(this.transform.position, new(lineRenderer.startWidth / 2.0f, lineRenderer.startWidth / 2.0f),
        (float)Math.Atan2(direction.y, direction.x) * 57.3f, this.direction, 1000.0f, playerAndBarrierMask);

        Vector3[] endPoints = { this.transform.position, this.transform.position };
        if (boxCast)
        {
            endPoints[1] = (Vector3)boxCast.point;
            if (boxCast.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("hit player");
                StartCoroutine(boxCast.collider.gameObject.GetComponent<Player>().Die());
            }
        }
        else
        {
            endPoints[1] = this.transform.position + ((Vector3)direction * 1000.0f);
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
}
