using Unity.VisualScripting;
using UnityEngine;

public abstract class PressurePlate : MonoBehaviour
{
    [SerializeField] protected int requiredBatteries = 0;
    [SerializeField] protected int numBatteries;
    [SerializeField] SpriteRenderer spriteRenderer;

    /// <summary>
    /// Triggers the door when something steps on the plate.
    /// </summary>
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject != Player.ActivePlayer.gameObject) return;
        spriteRenderer.transform.Translate(0.0f, -0.1f, 0.0f);
        OnPress();
    }
    /// <summary>
    /// Triggers the door when something gets off the plate.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject != Player.ActivePlayer.gameObject) return;
        spriteRenderer.transform.Translate(0.0f, 0.1f, 0.0f);
        OnRelease();
    }

    protected abstract void OnPress();
    protected abstract void OnRelease();
}
