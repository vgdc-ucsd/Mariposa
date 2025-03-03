using UnityEngine;

public class Passcode : MonoBehaviour
{
    public string correctPasscode = "1234"; //temp passcode
    private string enteredPasscode = "";

    public Door passcodeDoor;

    /// <summary>
    /// Call this function when a button is pressed
    /// </summary>
    public void EnterDigit(string digit)
    {
        if (enteredPasscode.Length < correctPasscode.Length)
        {
            enteredPasscode += digit;
        }
    }

    /// <summary>
    /// Call this function when the submit button is pressed
    /// </summary>
    public void SubmitPasscode()
    {
        if (enteredPasscode == correctPasscode)
        {
            passcodeDoor.ToggleLock(); // Unlock door
            passcodeDoor.ChangeState(); // Open door
        }
        else
        {
            Debug.Log("Incorrect passcode!");
        }
        enteredPasscode = ""; // Reset after submission
    }

    /// <summary>
    /// Call this function when the clear button is pressed
    /// </summary>
    public void ClearPasscode()
    {
        enteredPasscode = "";
    }
}
