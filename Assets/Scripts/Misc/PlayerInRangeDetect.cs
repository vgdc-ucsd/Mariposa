using UnityEngine;

public class PlayerInRangeDetect : MonoBehaviour
{

    [Header("View Range")]
    [SerializeField] [Range(1, 30)] float sizeX;
    [SerializeField] [Range(1, 30)] float sizeY;
    [SerializeField] [Range(0, 10)] float offsetX;
    [SerializeField] [Range(0, 10)] float offsetY;

    private Player player;

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
    }

    public bool IsPlayerInRange()
    {
        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;
        float minX = transform.position.x - offsetX - (sizeX / 2);
        float maxX = transform.position.x + offsetX + (sizeX / 2);
        float minY = transform.position.y - offsetY - (sizeX / 2);
        float maxY = transform.position.y + offsetY + (sizeY / 2);
        bool isPlayerInRange = (playerX > minX && playerX < maxX) && (playerY > minY && playerY < maxY);
        return isPlayerInRange;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && IsPlayerInRange()) Gizmos.color = Color.red;
        Vector3 center = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, 0);
        Vector3 size = new Vector3(sizeX, sizeY, 0);
        Gizmos.DrawWireCube(center, size);
    }
}
