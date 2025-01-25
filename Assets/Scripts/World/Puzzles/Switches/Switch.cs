using UnityEngine;

/// <summary>
/// Interactable object that toggles something on or off. Includes buttons and pressure plates.
/// </summary>
public abstract class Switch : MonoBehaviour
{

    /// <summary>
    /// Whether this switch has been toggled on or off.
    /// To invert the toggled state of this switch, do something like SwitchToggled = !SwitchToggled
    /// </summary>
    public bool SwitchToggled;

    /// <summary>
    /// Activate the switch.
    /// This is where we invert the toggled state of the switch, as well as play sound effects and update the sprite of the switch.
    /// </summary>
    public abstract void TriggerSwitch();

    
}



