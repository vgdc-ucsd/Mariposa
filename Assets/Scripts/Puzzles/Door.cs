using UnityEngine;

public class Door : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isOpen;
    [SerializeField] private bool isLocked;
    [SerializeField] private Sprite openSprite, closedSprite;

    /// <summary>
    /// Finds the current collider and sets it to boxCollider.
    /// Box collider subject to change depending on how doors will open and close.
    /// (i.e. if it moves up and down or trigger is enabled and disabled) 
    /// </summary>
    void Awake()
    {
        Debug.Log("gimme");
        if (GetComponent<Collider2D>() == null)
        {
            Debug.Log("No Collider was found!");
            return;
        }

        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (isOpen) Open();
        SetSprite();
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

        SetSprite();
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
    /// Currently, the door opens by updating its LayerMask to not be Barrier.
    /// </summary>
    private void Open()
    {
        Debug.Log("Open");
        
        if (boxCollider == null) return;

        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    /// <summary>
    /// Closes the door disabling the player the ability to pass through.
    /// Currently, the door closes by updating its LyaerMask to be Barrier.
    /// </summary>
    private void Close()
    {
        Debug.Log("Close");
        
        if (boxCollider == null) return;

        gameObject.layer = LayerMask.NameToLayer("Barrier");
    }
    
    /// <summary>
    /// Sets the sprite to be open or closed.
    /// IDEALLY, this method is only called on startup. When the door opens or closes, the animation should play.
    /// </summary>
    private void SetSprite()
    {
        if (openSprite == null || closedSprite == null)
        {
            Debug.Log("Door sprite is missing!");
            return;
        }
        if (isOpen) spriteRenderer.sprite = openSprite;
        else spriteRenderer.sprite = closedSprite;
    }

    // If the door isn't locked, make it automatically open/close when player walks by.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isLocked || isOpen) return;
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            ChangeState();
            Debug.Log("opened automatically");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isLocked || !isOpen) return;
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            ChangeState();
            Debug.Log("closed automatically");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }
}
