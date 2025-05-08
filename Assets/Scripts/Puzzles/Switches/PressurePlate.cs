using Unity.VisualScripting;
using UnityEngine;

public abstract class PressurePlate : MonoBehaviour
{
    /// <summary>
    /// Triggers the door when something steps on the plate.
    /// </summary>
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject != Player.ActivePlayer.gameObject) return;
        OnPress();
    }
    /// <summary>
    /// Triggers the door when something gets off the plate.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject != Player.ActivePlayer.gameObject) return;
        OnRelease();
    }

    protected abstract void OnPress();
    protected abstract void OnRelease();
}
