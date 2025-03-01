using System.Collections;
using UnityEngine;

/// <summary>
/// Class for a Fruit Scale object to be used in the Fruit Scale Puzzle.
/// </summary>
public class FruitScale : MonoBehaviour
{
    private const float OFFSET_MULTIPLIER = 0.75f;
    private const float MOVE_SPEED = 4f;

    [SerializeField]
    [Range(FruitScalePuzzle.FRUIT_MIN_WEIGHT, FruitScalePuzzle.FRUIT_MAX_WEIGHT)]
    private int scaleValue;
    public int ScaleValue
    {
        get => scaleValue;
        set { scaleValue = value; }
    }
    public bool IsStorage
    { get; set; }
    [SerializeField] private Fruit fruit;
    public Fruit Fruit
    {
        get => fruit;
        set { fruit = value; }
    }
    public BoxCollider2D BoxCollider2D
    { get; set; }

    /// <summary>
    /// Initialize collider reference and set initial Y position
    /// </summary>
    public void InitializeScale()
    {
        BoxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        BoxCollider2D.enabled = false;
        SetPositionY();
    }

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
    /// Calculates the difference between the scale's value and the weight of 
    /// the fruit on it. Can be used for determining visual offset when a fruit 
    /// is placed.
    /// </summary>
    /// <returns>The difference between the scale's value and weight of the fruit</returns>
    public int CalcWeightDiff()
    {
        if(fruit == null) return scaleValue;
        else return scaleValue - fruit.Weight;
    }

    /// <summary>
    /// Immediately sets the vertical position of the scale to show its balance.
    /// </summary>
    public void SetPositionY()
    {
        transform.localPosition = new Vector3
        (
            transform.localPosition.x,
            Mathf.Max(CalcWeightDiff() * OFFSET_MULTIPLIER, -1 * OFFSET_MULTIPLIER),
            transform.localPosition.z
        );
    }

    /// <summary>
    /// Moves the scale to its new position over time.
    /// </summary>
    /// <returns></returns>
    public IEnumerator LerpPositionY()
    {
        Vector3 target = new Vector3
        (
            transform.localPosition.x,
            Mathf.Max(CalcWeightDiff() * OFFSET_MULTIPLIER, -1 * OFFSET_MULTIPLIER),
            transform.localPosition.z
        );

        while(Vector3.Distance(transform.localPosition, target) > 0.001f)
        {
            var step = MOVE_SPEED * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, step);

            yield return null;
        }

        transform.localPosition = target;
    }
}
