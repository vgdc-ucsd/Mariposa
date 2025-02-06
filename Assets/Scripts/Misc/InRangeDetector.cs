using UnityEngine;

public class InRangeDetect : MonoBehaviour
{
    public GameObject Target => target;

    [Header("Target GameObject")]
    [SerializeField] GameObject target;

    [Header("View Range")]
    [SerializeField] [Range(1, 30)] float sizeX;
    [SerializeField] [Range(1, 30)] float sizeY;
    [SerializeField] [Range(0, 10)] float offsetX;
    [SerializeField] [Range(0, 10)] float offsetY;

    public bool IsTargetInRange()
    {
        float targetX = target.transform.position.x;
        float targetY = target.transform.position.y;
        float minX = transform.position.x - offsetX - (sizeX / 2);
        float maxX = transform.position.x + offsetX + (sizeX / 2);
        float minY = transform.position.y - offsetY - (sizeX / 2);
        float maxY = transform.position.y + offsetY + (sizeY / 2);
        bool isPlayerInRange = (targetX > minX && targetX < maxX) && (targetY > minY && targetY < maxY);
        return isPlayerInRange;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && IsTargetInRange()) Gizmos.color = Color.red;
        Vector3 center = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, 0);
        Vector3 size = new Vector3(sizeX, sizeY, 0);
        Gizmos.DrawWireCube(center, size);
    }
}
