using UnityEngine;

public class RadialRangeDetector : MonoBehaviour, IRangeDetector
{
    public GameObject Target => target;

    [Header("Target GameObject")]
    [SerializeField] GameObject target;

    [Header("View Range")]
    [SerializeField, Range(1, 30)] float range;
    [SerializeField, Range(-10, 10)] float offsetX;
    [SerializeField, Range(-10, 10)] float offsetY;

    [Header("Gizmo Color")]
    [SerializeField] Color inRange;
    [SerializeField] Color notInRange;

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public bool IsTargetInRange()
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(offsetX, offsetY);
        return Vector2.Distance(origin, target.transform.position) <= range;
    }

    public float GetMaxLength(Vector2 direction)
    {
        return range;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = target != null && IsTargetInRange()
            ? inRange
            : notInRange;
        Vector3 center = new(transform.position.x + offsetX, transform.position.y + offsetY, 0);
        Gizmos.DrawWireSphere(center, range);
    }
}
