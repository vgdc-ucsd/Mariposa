using UnityEngine;

public class Door : MonoBehaviour
{
    private SpriteRenderer sprite; //Temp to indicate visually the state of the door
    
    private Rigidbody2D rb;
    [SerializeField] private bool isOpen;
    [SerializeField] private bool isLocked;

    /// <summary>
    /// Finds the current rigid body and sets it to rb.
    /// </summary>
    void Awake()
    {
        if (GetComponent<Rigidbody2D>() == null)
        {
            Debug.Log("No Rigid Body was found!");
            return;
        }
        if (GetComponent<SpriteRenderer>() == null)
        {
            Debug.Log("No Sprite Renderer was found!");
            return;
        }
        
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
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
        
        if (isLocked) Debug.Log("Door is Locked");
        
        if (!isLocked) Debug.Log("Door is unlocked");
    }
    
    /// <summary>
    /// Opens the door allowing the player to pass through.
    /// </summary>
    private void Open()
    {
        Debug.Log("Open");
        
        if (rb == null) return;
        
        if (sprite == null) return;

        rb.simulated = false;
        sprite.color = new Color(0.5f, 0.5f, 0.5f, 1f);
    }

    /// <summary>
    /// Closes the door disabling the player the ability to pass through.
    /// </summary>
    private void Close()
    {
        Debug.Log("Close");
        
        if (rb == null) return;
        
        if (sprite == null) return;

        rb.simulated = true;
        sprite.color = new Color(1f, 1f, 1f, 1f);
    }
}
