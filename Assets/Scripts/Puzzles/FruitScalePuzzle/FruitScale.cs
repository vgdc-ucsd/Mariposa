using UnityEngine;

/// <summary>
/// Class for a Fruit Scale object to be used in the Fruit Scale Puzzle. 
/// Contains instance variables for the scale's value and the fruit placed on 
/// the scale.
/// </summary>
public class FruitScale : MonoBehaviour
{
    [SerializeField]
    [Range(FruitScalePuzzle.FRUIT_MIN_WEIGHT, FruitScalePuzzle.FRUIT_MAX_WEIGHT)]
    private int value;
    [SerializeField] private Fruit fruit; // TODO: Serialized as a debug, hide later

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
}
