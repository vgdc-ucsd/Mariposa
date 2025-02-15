using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Editor;
using UnityEngine.Timeline;

public class WaterPuzzle : Puzzle
{
    public static WaterPuzzle Instance;
    public GameObject PuzzleUI;
    [HideInInspector] public WaterPuzzleTile StartTile, EndTile;
    [HideInInspector] public WaterPuzzleTile[,] Tiles;
    public int GridWidth, GridHeight;
    [HideInInspector] public bool PuzzleComplete;

    [SerializeField] private GameObject tilePrefab;
    private List<WaterPuzzleTile> tilesInSolution;

    public float RandomTurnChance; // odds from 0 to 1 for the solution generator to make a random turn between tiles.
    public float TileWidth, TileHeight; // width and height of one tile in the scene
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

        // move the puzzle object so that the center of the puzzle is at (0, 0, 0) 
        PuzzleUI.transform.position = new Vector3(TileWidth * -GridWidth / 2, TileHeight * GridHeight / 2, 0);

        Tiles = new WaterPuzzleTile[GridWidth, GridHeight];
        for (int i = 0; i < GridWidth; i++)
        {
            for (int j = 0; j < GridHeight; j++)
            {
                GameObject tile = Instantiate(tilePrefab,  new Vector3(TileWidth * i, -TileHeight * j, 0), Quaternion.identity, PuzzleUI.transform);
                tile.transform.position += PuzzleUI.transform.position;
                Tiles[i, j] = tile.GetComponent<WaterPuzzleTile>();
                Tiles[i, j].PosX = i;
                Tiles[i, j].PosY = j;
                Tiles[i, j].InitializeTile();
            }
        }
        StartTile = Tiles[0, GridHeight / 2];
        EndTile = Tiles[GridWidth - 1, GridHeight / 2];
        /*foreach (GameObject tile in tilesFound)
        {
            WaterPuzzleTile wpt = tile.GetComponent<WaterPuzzleTile>();
            tiles[wpt.posX, wpt.posY] = wpt;
        }*/

        GenerateSolution();
    }


    private void OnEnable()
    {
        ActivatePuzzle();
    }

    public void ActivatePuzzle()
    {
        PuzzleUI.SetActive(true);
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

    public void CompletePuzzle()
    {
        OnComplete();
        PuzzleComplete = true;
    }
    
    /// <summary>
    /// Procedurally generates a solution for the puzzle.
    /// Set RandomTurnChance to a value close to 1 for more turns in the solution path,
    /// or set it to a value close to 0 for fewer turns.
    /// </summary>
    public void GenerateSolution()
    {
        tilesInSolution = new List<WaterPuzzleTile>();
        tilesInSolution.Add(StartTile);
        int[] direction = { 0, 1 };
        int[] right = { 1, 0 };
        int[] up = { 0, -1 };
        int[] left = { -1, 0 };
        int[] down = { 0, 1 };
        int x = StartTile.PosX;
        int y = StartTile.PosY;
        while (!tilesInSolution.Contains(EndTile))
        {

            int[] prevDirection = { direction[0], direction[1] };
            bool turned = false;
            if ((y == 0 || y == GridHeight - 1) && (direction[1] != 0))// direction should be up or down, set it to right
            {
                right.CopyTo(direction, 0);
                turned = true;
            }
            else if (x == EndTile.PosX && direction[0] == 1) // direction should be right, set it to up or down
            {
                if (y < EndTile.PosY) down.CopyTo(direction, 0);
                else up.CopyTo(direction, 0);
                turned = true;
            }
            else if (x == StartTile.PosX && y != StartTile.PosY && direction[0] == -1) // direction should be left, set it to up or down
            {
                if (y > StartTile.PosY) down.CopyTo(direction, 0);
                else up.CopyTo(direction, 0);
                turned = true;
            }
            else if (Random.Range(0f, 1f) < RandomTurnChance)
            {
                
                TurnDirection(direction).CopyTo(direction, 0);
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    TurnDirection(direction).CopyTo(direction, 0);
                    TurnDirection(direction).CopyTo(direction, 0);
                }

                if (y == 0) down.CopyTo(direction, 0);
                if (y == GridHeight - 1) up.CopyTo(direction, 0);

                turned = true;
            }

            // if the next tile is already in the solution, try to turn to avoid that tile.
            // if 4 turns are made without finding an empty tile, then go through the tile anyway
            int turnsMade = 0;
            while (!IsTileFree(x + direction[0], y + direction[1]))
            {
                TurnDirection(direction).CopyTo(direction, 0);
                turned = !turned;

                turnsMade++;
                if (turnsMade == 4)
                {
                    break;
                }
            }

            if ((direction[0] == 1 && prevDirection[0] == -1)
                || (direction[0] == -1 && prevDirection[0] == 1)
                || (direction[1] == -1 && prevDirection[1] == 1)
                || (direction[1] == 1 && prevDirection[1] == -1)) continue;


            
            if (IsTileInBounds(x, y))
            {
                if (turned) Tiles[x, y].MustBeTurn = true;
                else Tiles[x, y].MustBeStraight = true;
                x += direction[0];
                y += direction[1];
                if (!tilesInSolution.Contains(Tiles[x, y])) tilesInSolution.Add(Tiles[x, y]);
                else Tiles[x, y].MustBeCross = true;
            }








        }
    }

    private bool IsTileFree(int x, int y)
    {
        return (IsTileInBounds(x, y) && !tilesInSolution.Contains(Tiles[x, y]));
    }

    private bool IsTileInBounds(int x, int y)
    {
        return (x >= 0 && x < GridWidth && y >= 0 && y < GridHeight);
    }

    private int[] TurnDirection(int[] direction)
    {
        if (direction[0] == 1 && direction[1] == 0) return new int[] { 0, -1 };
        else if (direction[0] == 0 && direction[1] == -1) return new int[] { -1, 0 };
        else if (direction[0] == -1 && direction[1] == 0) return new int[] { 0, 1 };
        else if (direction[0] == 0 && direction[1] == 1) return new int[] { 1, 0 };
        else return new int[] { 0, 0 };
        
    }

    
}
