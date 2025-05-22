using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;

public class BillboardPuzzle : Puzzle
{
    public static BillboardPuzzle Instance;
    public BillboardPuzzleTile[,] board;
    public GameObject[] boardDisplay;
    public GameObject[] miniBoardDisplay;
    private int width = 5;
    private int height = 3;
    private List<int> endXValues = new List<int>();
    private List<int> endRotValues = new List<int>();
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject interactable;
    [HideInInspector] public bool Initialized;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Tried to create more than one instance of the BillboardPuzzle singleton!");
        }
        Initialize();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) PuzzlePopupManager.Instance.TryHidePuzzle();
    }

    public void Initialize()
    {
        board = new BillboardPuzzleTile[height, width];
        int index = 0;
        GenerateSolution();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {

                board[i, j].x = j;
                board[i, j].endX = endXValues[index];
                board[i, j].spriteIndex = board[i, j].endX + i * width;
                board[i, j].rotation = endRotValues[index];
                index++;
            }
        }
        UpdatePuzzleDisplay();
        Initialized = true;
    }

    [ContextMenu("Reset Puzzle")]
    private new void Reset()
    {
        base.Reset();
        int index = 0;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {

                board[i, j].x = j;
                board[i, j].endX = endXValues[index];
                board[i, j].spriteIndex = board[i, j].endX;
                board[i, j].rotation = endRotValues[index];
                index++;
            }
        }
        UpdatePuzzleDisplay();
    }

    public void ShiftRow(int row)
    {
        for (int i = 0; i < width; i++)
        {
            board[row, i].x++;
            board[row, i].x %= width;
        }
        UpdatePuzzleDisplay();
    }


    public void RotateCol(int col)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (board[i, j].x != col) continue;
                if (col % 2 == 0) board[i, j].rotation++;
                else board[i, j].rotation--;
                board[i, j].rotation %= 4;
            }
        }
        UpdatePuzzleDisplay();
    }

    /// <summary>
    /// Procedurally generates a solution to the puzzle.
    /// To prevent the puzzle from being mathematically impossible, the sums of the rotation values of each row are made to be equal.
    /// </summary>
    private void GenerateSolution()
    {
        int targetSum = 10;
        for (int i = 0; i < height; i++)
        {
            int rotSum = 0;
            int[] rotRow = new int[width];
            int shiftX = Random.Range(0, width);
            List<int> rowXValues = new List<int>();
            for (int j = 0; j < width; j++)
            {
                rowXValues.Add(j);
                int rot = Random.Range(0, 4);
                rotRow[j] = rot;
                rotSum += rot;
            }

            while (rotSum != targetSum)
            {
                int rIndex = Random.Range(0, width);
                rotRow[rIndex]++;
                rotSum++;
                if (rotRow[rIndex] == 4)
                {
                    rotRow[rIndex] = 0;
                    rotSum -= 4;
                }
            }
            Debug.Log(rotRow.Sum());
            Debug.Log(rotRow.ToCommaSeparatedString());

            foreach (int rot in rotRow)
            {
                endRotValues.Add(rot);
            }

            foreach (int xVal in rowXValues)
            {
                endXValues.Add((xVal + shiftX) % width);
            }
        }

        
    }

    private void UpdatePuzzleDisplay()
    {

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int index = board[i, j].x + width * i;
                boardDisplay[index].GetComponent<SpriteRenderer>().sprite = sprites[board[i, j].spriteIndex];
                boardDisplay[index].transform.eulerAngles = Vector3.forward * board[i, j].rotation * -90;
                if (miniBoardDisplay.Length == 0) continue;
                miniBoardDisplay[index].GetComponent<SpriteRenderer>().sprite = sprites[board[i, j].spriteIndex];
                miniBoardDisplay[index].transform.eulerAngles = Vector3.forward * board[i, j].rotation * -90;
            }
        }

        if (IsPuzzleComplete()) OnComplete();
    }

    private void OnDisable()
    {
        if (interactable)
        {
            interactable.SetActive(true);
            UpdatePuzzleDisplay();
        }
    }

    private bool IsPuzzleComplete()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (board[i, j].x != board[i, j].endX) return false;
                if (board[i, j].rotation != board[i, j].endRotation) return false;
            }
        }

        return true;
    }
}

public struct BillboardPuzzleTile
{
    public int x;
    public int endX;
    public int rotation;
    public int endRotation;
    public int spriteIndex;
}
