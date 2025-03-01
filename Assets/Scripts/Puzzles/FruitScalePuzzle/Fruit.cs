using UnityEngine;

/// <summary>
/// Class for a Fruit object to be used in the Fruit Scale Puzzle.
/// </summary>
public class Fruit : MonoBehaviour
{
    [SerializeField]
    [Range(FruitScalePuzzle.FRUIT_MIN_WEIGHT, FruitScalePuzzle.FRUIT_MAX_WEIGHT)]
    private int weight;
    public int Weight
    {
        get => weight;
        set { weight = value; }
    }
    [SerializeField] private FruitScale scale;
    public FruitScale Scale
    {
        get => scale;
        set
        {
            scale = value;
            transform.position = scale.transform.position;
            transform.SetParent(scale.transform);
        }
    }
    private SpriteRenderer fruitSR;

    void Awake()
    {
        fruitSR = GetComponent<SpriteRenderer>();
        if(fruitSR == null) Debug.LogWarning("Fruit sprite renderer not found!");
    }

    /// <summary>
    /// When the player clicks on the fruit, select it and set its origin scale.
    /// </summary>
    public void OnMouseDown()
    {
        if(!FruitScalePuzzle.Instance.IsComplete)
        {
            FruitScalePuzzle.Instance.SelectedFruit = this;
            FruitScalePuzzle.Instance.OldScale = scale;
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
            FruitScalePuzzle.Instance.OldScale = scale;
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
            bool fruitPlaced = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPos, 0.01f);
            foreach(Collider2D collider in colliders)
            {
                FruitScale Scale = collider.GetComponent<FruitScale>();
                if(Scale != null)
                {
                    Scale.TryPlaceFruit();
                    fruitPlaced = true;
                    break;
                }
            }

            if(!fruitPlaced) FruitScalePuzzle.Instance.SelectedFruit = null;
            transform.localPosition = Vector3.zero;
        }
    }

    /// <summary>
    /// Set this fruit as the selected fruit and update visuals
    /// </summary>
    public void Select()
    {
        fruitSR.color = Color.white * 0.7f;
    }

    /// <summary>
    /// Deselect the fruit and update visuals
    /// </summary>
    public void Deselect()
    {
        fruitSR.color = Color.white;
    }
}
