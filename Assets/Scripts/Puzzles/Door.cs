using UnityEngine;

public class Door : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private bool isOpen;
    private bool isLocked;

    /// <summary>
    /// Finds the current collider and sets it to boxCollider.
    /// Box collider subject to change depending on how doors will open and close.
    /// (i.e. if it moves up and down or trigger is enabled and disabled) 
    /// </summary>
    void Awake()
    {
        if (GetComponent<Collider2D>() == null)
        {
            Debug.Log("No Collider was found!");
            return;
        }
        
        boxCollider = GetComponent<BoxCollider2D>();
    }
    /// <summary>
    /// Function to call when something external wants to open or
    /// close the door. ChangeState() toggles the isOpen bool then
    /// checks to run Open() or Close(). If the Door is locked
    /// the door will not open/close.
    /// </summary>
    public void ChangeState()
    {
        if (isLocked)
        {
            Debug.Log("Door is Locked");
            return;
        }
        
        isOpen = !isOpen;
        
        if(isOpen) Open();
        
        if(!isOpen) Close();
        
        Debug.Log("Door has changed open state");
    }

    /// <summary>
    /// Calling this function toggles the lock on the door.
    /// </summary>
    public void ToggleLock()
    {
        isLocked = !isLocked;
        Debug.Log("Door has changed locked state");
    }
    
    /// <summary>
    /// Opens the door allowing the player to pass through.
    /// Currently, the door opens by changing the collider to a trigger.
    /// </summary>
    private void Open()
    {
        Debug.Log("Open");
        boxCollider.isTrigger = true;
    }

    /// <summary>
    /// Closes the door disabling the player the ability to pass through.
    /// Currently, the door closes by changing the collider to not be a trigger.
    /// </summary>
    private void Close()
    {
        Debug.Log("Close");
        boxCollider.isTrigger = false;
    }
}
