using UnityEngine;

public class InRangeDetector : MonoBehaviour
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

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            if (IsTargetInRange()) Gizmos.color = inRange;
            else Gizmos.color = notInRange;
        }
        Vector3 center = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, 0);
        Vector3 size = new Vector3(sizeX, sizeY, 0);
        Gizmos.DrawWireCube(center, size);
    }
}
