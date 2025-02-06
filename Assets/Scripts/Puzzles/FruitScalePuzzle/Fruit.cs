using UnityEngine;

/// <summary>
/// Class for a Fruit object to be used in the Fruit Scale Puzzle.
/// </summary>
public class Fruit : MonoBehaviour
{
    [SerializeField]
    [Range(FruitScalePuzzle.FRUIT_MIN_WEIGHT, FruitScalePuzzle.FRUIT_MAX_WEIGHT)]
    private int weight;
    [SerializeField] private FruitScale Scale;
    [SerializeField] private Color DebugColor; // TODO: for debug, remove later
    private SpriteRenderer fruitSR;

    void Awake()
    {
        if(Scale != null) Scale.GetComponent<BoxCollider2D>().enabled = false;
        fruitSR = GetComponent<SpriteRenderer>();
        if(fruitSR == null) Debug.LogWarning("Fruit sprite renderer not found!");
    }

    public int GetWeight() { return weight; }
    public void SetScale(FruitScale scale) { this.Scale = scale; }

    /// <summary>
    /// When the player clicks on the fruit, select it and set its origin scale.
    /// </summary>
    public void OnMouseDown()
    {
        if(!FruitScalePuzzle.Instance.IsComplete)
        {
            SelectThis();
            FruitScalePuzzle.Instance.OldScale = Scale;
        }
    }

    /// <summary>
    /// While the player is dragging the fruit, move the sprite with the cursor
    /// </summary>
    public void OnMouseDrag()
    {
        if(!FruitScalePuzzle.Instance.IsComplete)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(worldPos.x, worldPos.y, 0);
            FruitScalePuzzle.Instance.OldScale = Scale;
        }
    }

    /// <summary>
    /// When the player releases the fruit, if the cursor is over a scale, try 
    /// to place the fruit there. If not, return it to its origin scale.
    /// </summary>
    public void OnMouseUp()
    {
        if(!FruitScalePuzzle.Instance.IsComplete)
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPos, 0.01f);
            foreach(Collider2D collider in colliders)
            {
                FruitScale Scale = collider.GetComponent<FruitScale>();
                if(Scale != null)
                {
                    Scale.TryPlaceFruit();
                    break;
                }
            }

            if(Scale != null) transform.position = Scale.transform.position;
            else transform.localPosition = Vector3.zero;
        }
    }

    /// <summary>
    /// Set this fruit as the selected fruit and update visuals
    /// </summary>
    private void SelectThis()
    {
        if(FruitScalePuzzle.Instance.SelectedFruit != null)
        {
            FruitScalePuzzle.Instance.SelectedFruit.Deselect();
        }
        FruitScalePuzzle.Instance.SelectedFruit = this;

        // fruitSR.color = Color.white * 0.7f;
        fruitSR.color = DebugColor * 0.7f; // TODO: for debug, remove later
    }

    /// <summary>
    /// Deselect the fruit and update visuals
    /// </summary>
    public void Deselect()
    {
        // fruitSR.color = Color.white;
        fruitSR.color = DebugColor; // TODO: for debug, remove later
    }
}
