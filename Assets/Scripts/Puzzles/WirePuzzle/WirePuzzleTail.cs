using UnityEngine;

public class WirePuzzleTail : MonoBehaviour
{
    public WirePuzzleWire Wire;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;

    public void InitializeWireTail()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = new(Wire.Color.r, Wire.Color.g, Wire.Color.b, 0.5f);
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public Vector3 GetConnectedPosition(int layer)
    {
        return new Vector3(transform.position.x, transform.position.y, -1 * layer);
    }
}
