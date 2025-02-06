using UnityEngine;

/// <summary>
/// Class for a Fruit Scale object to be used in the Fruit Scale Puzzle.
/// </summary>
public class FruitScale : MonoBehaviour
{
    private const float OFFSET_MULTIPLIER = 0.75f;

    [SerializeField]
    [Range(FruitScalePuzzle.FRUIT_MIN_WEIGHT, FruitScalePuzzle.FRUIT_MAX_WEIGHT)]
    private int value;
    [SerializeField] private bool isStorage;
    [SerializeField] private Fruit fruit;

    public bool IsStorage() { return isStorage; }

    /// <summary>
    /// Tries to place a fruit on this scale
    /// </summary>
    public void TryPlaceFruit()
    {
        if(FruitScalePuzzle.Instance.SelectedFruit != null)
        {
            FruitScalePuzzle.Instance.PlaceFruit(this);
        }
    }

    /// <summary>
    /// Setter for the fruit instance variable.
    /// </summary>
    /// <param name="fruit">The fruit placed on the scale</param>
    public void SetFruit(Fruit fruit)
    {
        this.fruit = fruit;
    }

    /// <summary>
    /// Calculates the difference between the scale's value and the weight of 
    /// the fruit on it. Can be used for determining visual offset when a fruit 
    /// is placed.
    /// </summary>
    /// <returns>The difference between the scale's value and weight of the fruit</returns>
    public int CalcWeightDiff()
    {
        if(fruit == null) return value;
        else return value - fruit.GetWeight();
    }

    /// <summary>
    /// Sets the vertical position of the scale to show its balance. May be 
    /// replaced with a coroutine for smooth movement.
    /// </summary>
    public void SetPositionY()
    {
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            Mathf.Max(CalcWeightDiff() * OFFSET_MULTIPLIER, -1),
            transform.localPosition.z
        );
    }
}
