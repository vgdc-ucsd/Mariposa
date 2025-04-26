using UnityEngine;

public class Button : Switch
{
    [SerializeField] private Door door;
    [SerializeField] private bool isPressed;
    
    /// <summary>
    /// Function to call when pressing the button.
    /// Button is designed to be pressed only once.
    /// </summary>
    public override void TriggerSwitch()
    {
        Debug.Log("Button was Pressed");

        if (isPressed) return;
        
        SwitchToggled = !SwitchToggled;
        
        if (door != null) door.ChangeState();
        
        isPressed = true;
    }
}
