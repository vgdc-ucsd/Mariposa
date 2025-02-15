using UnityEngine;

public class WaterPuzzle : Puzzle
{
    public static WaterPuzzle Instance;
    public GameObject puzzleUI;
    public WaterPuzzleTile startTile, endTile;
    public WaterPuzzleTile[,] tiles;
    public int gridWidth, gridHeight;
    public bool puzzleComplete;

    /// <summary>
    /// Different sprites for the water tiles, depending on how many pipes they have
    /// <code>
    /// 0 - no pipes
    /// 1 - 1 pipe facing right
    /// 2 - 2 pipes right and up
    /// 3 - 2 pipes right and left
    /// 4 - 3 pipes right down and up
    /// 5 - 4 pipes in a + shape
    /// </code>
    /// Although there are more possible tile shapes, they can be represented by rotating these sprites
    /// </summary>
    public Sprite[] tileSprites;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Tried to create more than one instance of the WaterPuzzle singleton!");
        }
        tiles = new WaterPuzzleTile[gridWidth, gridHeight];

        
    }

    private void OnEnable()
    {
        ActivatePuzzle();
    }

    public void ActivatePuzzle()
    {
        puzzleUI.SetActive(true);
    }

    public void ResetPuzzle()
    {
        foreach (WaterPuzzleTile tile in Instance.tiles)
        {
            if (tile == null) continue;
            tile.EmptyTile();
        }
        startTile.FillTile(true);
    }

    public void CompletePuzzle()
    {
        OnComplete();
        puzzleComplete = true;
    }
    
}
