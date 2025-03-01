using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI; // Import UI for Text

public class Passcode : MonoBehaviour
{
    [SerializeField] private string correctPasscode = "1111"; // Temp passcode
    private string enteredPasscode = "";

    public Door passcodeDoor;
    public GameObject keypadPanel; // Reference to the keypad UI

    private void Start()
    {
        if (keypadPanel != null)
            keypadPanel.SetActive(false); // Ensure it's hidden initially
    }

    public void ShowKeypad()
    {
        if (keypadPanel != null)
            keypadPanel.SetActive(true); // Show keypad
    }

    public void HideKeypad()
    {
        if (keypadPanel != null)
            keypadPanel.SetActive(false); // Hide keypad
    }

    /// <summary>
    /// Call this function when a button is pressed
    /// </summary>
    public void EnterDigit(Text buttonText)
    {
        if (buttonText != null)
        {
            string digit = buttonText.text;
            if (enteredPasscode.Length < correctPasscode.Length)
            {
                enteredPasscode += digit;
                Debug.Log("Entered Passcode: " + enteredPasscode);
            }
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
            Debug.Log("Correct passcode!");
        }
        else
        {
            Debug.Log("Incorrect passcode!");
        }
        enteredPasscode = ""; // Reset after submission
        HideKeypad();
    }

    /// <summary>
    /// Call this function when the clear button is pressed
    /// </summary>
    public void ClearPasscode()
    {
        enteredPasscode = "";
    }
}
