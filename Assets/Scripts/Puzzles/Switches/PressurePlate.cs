using Unity.VisualScripting;
using UnityEngine;

public abstract class PressurePlate : MonoBehaviour
{
    [SerializeField] private int requiredBatteries = 0;
    [SerializeField] SpriteRenderer spriteRenderer;

    /// <summary>
    /// Triggers the door when something steps on the plate.
    /// </summary>
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject != Player.ActivePlayer.gameObject) return;
        spriteRenderer.transform.localPosition = new Vector2(0, -0.25f);
        OnPress();
    }
    /// <summary>
    /// Triggers the door when something gets off the plate.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject != Player.ActivePlayer.gameObject) return;
        spriteRenderer.transform.localPosition = new Vector2(0, 0);
        OnRelease();
    }

    protected abstract void OnPress();
    protected abstract void OnRelease();
}
