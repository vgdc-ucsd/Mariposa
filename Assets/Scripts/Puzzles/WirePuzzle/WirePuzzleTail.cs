using UnityEngine;

public class WirePuzzleTail : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;

    public void InitializeWireTail()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public Vector3 GetConnectedPosition(int layer)
    {
        return new Vector3(transform.position.x, transform.position.y, -1 * layer);
    }
}
