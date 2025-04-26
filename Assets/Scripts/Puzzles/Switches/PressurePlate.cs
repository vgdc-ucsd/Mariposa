using UnityEngine;

public class PressurePlate : Switch
{
    [SerializeField] private Door door;
    
    /// <summary>
    /// Triggers the door when something steps on the plate.
    /// </summary>
    void OnCollisionEnter2D()
    {
        Debug.Log("Plate is pressed");
        TriggerSwitch();
    }
    /// <summary>
    /// Triggers the door when something gets off the plate.
    /// </summary>
    private void OnCollisionExit2D()
    {
        Debug.Log("Plate is released");
        TriggerSwitch();
    }

    /// <summary>
    /// Toggles the state of the door.
    /// </summary>
    public override void TriggerSwitch()
    {
        SwitchToggled = !SwitchToggled;
        if (door != null) door.ChangeState();
    }
}
