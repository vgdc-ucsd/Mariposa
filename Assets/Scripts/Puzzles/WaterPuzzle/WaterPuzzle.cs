using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WaterPuzzle : Puzzle
{
    [SerializeField] public bool twoEndings;

    //public static WaterPuzzle Instance;
    public GameObject PuzzleUI;
    [HideInInspector] public WaterPuzzleTile StartTile, EndTile, EndTile2;
    [HideInInspector] public WaterPuzzleTile[,] Tiles;
    public int GridWidth, GridHeight;
    [HideInInspector]
    
    [SerializeField] private GameObject tilePrefab;
    private List<WaterPuzzleTile> tilesInSolution;
    [SerializeField] private Transform content;
    [SerializeField] private int puzzleNumber = 1;

    public float RandomTurnChance; // odds from 0 to 1 for the solution generator to make a random turn between tiles.
    public Vector2 TileSize; // width and height of one tile in the scene
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
    public Sprite[] TileSprites;

    [HideInInspector] public bool UsedPipeSplitter, PipeSplitterToggled;
    [HideInInspector] public WaterPuzzleTile SplitTile;
    [HideInInspector] public bool[] SplitTilePipes = new bool[4];
    [SerializeField] private TMP_Text toggleText;

    [SerializeField] private bool usingSeedBank;
    [SerializeField] private int[] seedBank;

    public static readonly int2 right = new(1, 0);
    public static readonly int2 left = new(-1, 0);
    public static readonly int2 up = new(0, 1);
    public static readonly int2 down = new(0, -1);

    private void Awake()
    {
        // move the puzzle object so that the center of the puzzle is at (0, 0, 0)
        TileSize = PuzzleUI.GetComponent<RectTransform>().sizeDelta / new Vector2(GridWidth, GridHeight);
        PuzzleUI.GetComponent<GridLayoutGroup>().cellSize = TileSize;

        Tiles = new WaterPuzzleTile[GridWidth, GridHeight];
        for (int i = 0; i < GridHeight; i++)
        {
            for (int j = 0; j < GridWidth; j++)
            {
                GameObject tile = Instantiate(tilePrefab,  PuzzleUI.transform);
                Tiles[j, i] = tile.GetComponent<WaterPuzzleTile>();
                Tiles[j, i].PosX = j;
                Tiles[j, i].PosY = i;
                Tiles[j, i].InitializeTile();
                Tiles[j, i].puzzle = this;
            }
        }
        StartTile = Tiles[0, GridHeight / 2];
        if (twoEndings)
        {
            EndTile = Tiles[GridWidth - 1, GridHeight / 2 + 2];
            EndTile2 = Tiles[GridWidth - 1, GridHeight / 2 - 2];
        }
        else EndTile = Tiles[GridWidth - 1, GridHeight / 2];

        int seed = (int)System.DateTime.Now.Ticks;
        if (usingSeedBank) seed = seedBank[Random.Range(0, seedBank.Length)];
        Random.InitState(seed);
        GenerateSolution();
        UsedPipeSplitter = false;
    }

    public void ResetPuzzle()
    {
        foreach (WaterPuzzleTile tile in Tiles)
        {
            if (tile == null) continue;
            tile.EmptyTile();
        }

        StartTile.FillTile(true);
    }

    public IEnumerator CompletePuzzle()
    {
        OnComplete();
        yield return null;
    }

    public override void OnComplete()
    {
        switch (puzzleNumber)
        {
            case 1:
                WaterPuzzleProgressTracker.Instance.MarkPuzzle1Complete();
                break;
            case 2:
                WaterPuzzleProgressTracker.Instance.MarkPuzzle2Complete();
                break;
            case 3:
                WaterPuzzleProgressTracker.Instance.MarkPuzzle3Complete();
                break;
        }
        base.OnComplete();
    }
    
    public void RevertSplitTile()
    {
        if (SplitTile == null) return;
        foreach (WaterPuzzleTile tile in Tiles)
        {
            if (tile != null)
                tile.EmptyTile();
        }
        SplitTile.PipeRight = SplitTilePipes[0];
        SplitTile.PipeUp = SplitTilePipes[1];
        SplitTile.PipeLeft = SplitTilePipes[2];
        SplitTile.PipeDown = SplitTilePipes[3];
        SplitTile.SetSprite();
        SplitTile = null;
        SplitTilePipes = new bool[4];
        UsedPipeSplitter = false;
        toggleText.text = (PipeSplitterToggled ? "Click a pipe to split it!" : "Use Pipe Splitter");
        StartTile.FillTile(true);
    }

    public void ToggleSplitTile()
    {
        PipeSplitterToggled = !PipeSplitterToggled;
        if (UsedPipeSplitter)
        {
            toggleText.text = "Pipe Splitter already used";
            return;
        }
        toggleText.text = (PipeSplitterToggled ? "Click a pipe to split it!" : "Use Pipe Splitter");

    }

    /// <summary>
    /// Procedurally generates a solution for the puzzle.
    /// Set RandomTurnChance to a value close to 1 for more turns in the solution path,
    /// or set it to a value close to 0 for fewer turns.
    /// </summary>
    public void GenerateSolution(bool secondPass = false)
    {
        int x, y;
        int branchIndex = 0;
        if (!secondPass)
        {
            tilesInSolution = new List<WaterPuzzleTile>();
            tilesInSolution.Add(StartTile);

            x = StartTile.PosX;
            y = StartTile.PosY;
        }
        else
        {
            branchIndex = Random.Range(tilesInSolution.Count / 4, tilesInSolution.Count / 2);
            x = tilesInSolution[branchIndex].PosX;
            y = tilesInSolution[branchIndex].PosY;
        }
        int2 direction = Random.Range(0, 3) switch
        {
            0 => right,
            1 => up,
            2 => down,
            _ => throw new System.NotImplementedException()
        };

        int steps = 0;

        // first pass - needs end tile 1
        // second pass - needs end tile 2
        while (steps < GridWidth * GridHeight * 2)
        {
            if (!secondPass && tilesInSolution.Contains(EndTile)) break;
            else if (secondPass && tilesInSolution.Contains(EndTile2))
            {
                break;  
            }
            steps++;



            int2 prevDirection = direction;
            bool turned = false;
            if ((y == 0 || y == GridHeight - 1) && (direction.y != 0)) // direction should be up or down, set it to right
            {
                direction = right;
                turned = true;
            }
            else if (x == EndTile.PosX && direction.x == 1) // direction should be right, set it to up or down
            {
                if (y < EndTile.PosY) direction = down;
                else direction = up;
                turned = true;
            }
            else if (x == StartTile.PosX && y != StartTile.PosY && direction.x == -1) // direction should be left, set it to up or down
            {
                if (y > StartTile.PosY) direction = down;
                else direction = up;
                turned = true;
            }
            else if (Random.Range(0f, 1f) < RandomTurnChance)
            {

                direction = TurnDirection(direction, false);
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    direction = TurnDirection(direction, false);
                    direction = TurnDirection(direction, false);
                }

                if (y == 0) direction = down;
                if (y == GridHeight - 1) direction = up;

                turned = true;
            }

            // if the next tile is already in the solution, try to turn to avoid that tile.
            // if there's no free direction to go, attempt to turn right
            // also try to avoid making 180 degree turns if possible
            int turnsMade = 0;
            while (!IsTileFree(x + direction.x, y + direction.y, secondPass)
                || (direction.x == 1 && prevDirection.x == -1)
                || (direction.x == -1 && prevDirection.x == 1)
                || (direction.y == -1 && prevDirection.y == 1)
                || (direction.y == 1 && prevDirection.y == -1))
            {
                direction = TurnDirection(direction, false);
                turned = !turned;

                turnsMade++;
                if (turnsMade == 4)
                {
                    break;
                }
            }



            if (IsTileInBounds(x + direction.x, y + direction.y))
            {

                x += direction.x;
                y += direction.y;
                if (turned) Tiles[x - direction.x, y - direction.y].MustBeTurn = true;
                else Tiles[x - direction.x, y - direction.y].MustBeStraight = true;


                if (!tilesInSolution.Contains(Tiles[x, y]))
                {
                    tilesInSolution.Add(Tiles[x, y]);
                    Tiles[x, y].RandomPipeChance /= 20f;
                }
                else if (!twoEndings) Tiles[x, y].MustBeCross = true;


            }







        }

        if (twoEndings && !secondPass) GenerateSolution(true);
    }


    private bool IsTileFree(int x, int y, bool secondPass)
    {
        if (twoEndings && !secondPass && EndTile2.PosX == x && EndTile2.PosY == y) return false;
        return (IsTileInBounds(x, y) && !tilesInSolution.Contains(Tiles[x, y]));
    }

    private bool IsTileInBounds(int x, int y)
    {
        
        return (x >= 0 && x < GridWidth && y >= 0 && y < GridHeight);
    }

    private int2 TurnDirection(int2 direction, bool isClockwise)
    {
        if (direction.Equals(int2.zero)) return int2.zero;

        int2 rotMask = isClockwise ? new int2(1, -1) : new int2(-1, 1);
        return direction.yx * rotMask; // this swizzle combined with the rotMask is equivalent to applying the corresponding 90 degree rotation matrix
    }
}
