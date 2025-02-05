using UnityEngine;

public class Fruit : MonoBehaviour
{
    [SerializeField]
    [Range(FruitScalePuzzle.FRUIT_MIN_WEIGHT, FruitScalePuzzle.FRUIT_MAX_WEIGHT)]
    private int weight;

    public int GetWeight() { return weight; }
}
