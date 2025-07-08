using UnityEngine;

public class InRangeDetector : MonoBehaviour, IRangeDetector
{
    public GameObject Target => target;

    [Header("Target GameObject")]
    [SerializeField] GameObject target;

    [Header("View Range")]
    [SerializeField] [Range(1, 30)] float sizeX;
    [SerializeField] [Range(1, 30)] float sizeY;
    [SerializeField] [Range(-10, 10)] float offsetX;
    [SerializeField] [Range(-10, 10)] float offsetY;

    [Header("Gizmo Color")]
    [SerializeField] Color inRange;
    [SerializeField] Color notInRange;

    public float SizeX => sizeX;
    public float SizeY => sizeY;
    public float OffsetX => offsetX;
    public float OffsetY => offsetY;

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
	}

    public bool IsTargetInRange()
    {
        float targetX = target.transform.position.x;
        float targetY = target.transform.position.y;
        float minX = transform.position.x + offsetX - (sizeX / 2);
        float maxX = transform.position.x + offsetX + (sizeX / 2);
        float minY = transform.position.y + offsetY - (sizeY / 2);
        float maxY = transform.position.y + offsetY + (sizeY / 2);
        bool isPlayerInRange = (targetX > minX && targetX < maxX) && (targetY > minY && targetY < maxY);
        return isPlayerInRange;
    }

    public float GetMaxLength(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return 0; // No direction, no intersection

        direction.Normalize();

        float halfWidth = sizeX / 2f;
        float halfHeight = sizeY / 2f;

        float tMin = float.PositiveInfinity;

        // Edges of the rectangle
        float left   = offsetX - halfWidth;
        float right  = offsetX + halfWidth;
        float bottom = offsetY - halfHeight;
        float top    = offsetY + halfHeight;

        // Check intersection with each side of the rectangle
        // Horizontal sides (top/bottom)
        if (direction.y != 0)
        {
            float tTop = top / direction.y;
            float tBottom = bottom / direction.y;

            Vector2 hitTop = direction * tTop;
            Vector2 hitBottom = direction * tBottom;

            if (tTop > 0 && hitTop.x >= left && hitTop.x <= right)
                tMin = Mathf.Min(tMin, tTop);

            if (tBottom > 0 && hitBottom.x >= left && hitBottom.x <= right)
                tMin = Mathf.Min(tMin, tBottom);
        }

        // Vertical sides (left/right)
        if (direction.x != 0)
        {
            float tRight = right / direction.x;
            float tLeft = left / direction.x;

            Vector2 hitRight = direction * tRight;
            Vector2 hitLeft = direction * tLeft;

            if (tRight > 0 && hitRight.y >= bottom && hitRight.y <= top)
                tMin = Mathf.Min(tMin, tRight);

            if (tLeft > 0 && hitLeft.y >= bottom && hitLeft.y <= top)
                tMin = Mathf.Min(tMin, tLeft);
        }

        if (float.IsPositiveInfinity(tMin)) return 0; // No valid hit

        return tMin;
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            if (IsTargetInRange()) Gizmos.color = inRange;
            else Gizmos.color = notInRange;
        }
        Vector3 center = new(transform.position.x + offsetX, transform.position.y + offsetY, 0);
        Vector3 size = new(sizeX, sizeY, 0);
        Gizmos.DrawWireCube(center, size);
    }
}
