using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaterPuzzleTile : MonoBehaviour
{
    [HideInInspector] public WaterPuzzle puzzle;
    [HideInInspector] public bool PipeRight, PipeUp, PipeLeft, PipeDown;
    [HideInInspector] public bool HasWater;
    [HideInInspector] public int PosX, PosY;
    [HideInInspector] public BoxCollider2D MyCollider;
    public Image Image;
    [HideInInspector] public bool MustBeTurn;
    [HideInInspector] public bool MustBeStraight;
    [HideInInspector] public bool MustBeCross;
    [SerializeField] private Color fillColor;
    [SerializeField] private float popSpeed, popSizeScale, popRotSpeed, popEasingScale;
    private bool animating = false;
    public float RandomPipeChance; // odds from 0 to 1 per side on this tile for a pipe to randomly spawn there
    public GameObject background;
   
    private void Start()
    {
        if (puzzle.StartTile == this) FillTile(true);
        else EmptyTile();

        if (puzzle.EndTile == this || puzzle.EndTile2 == this) Image.color = Color.red;

        SetPipes();
        SetSprite();
    }

    /// <summary>
    /// Called right when this tile is created to initialize key variables.
    /// </summary>
    public void InitializeTile()
    {
        MyCollider = GetComponent<BoxCollider2D>();
        if (Image == null) Debug.LogError("Pipe Image component is missing!");
    }

    /// <summary>
    /// Assigns values to PipeRight, PipeUp, PipeLeft, and PipeDown, making sure that this tile can connect to adjacent tiles if it's part of the solution.
    /// </summary>
    private void SetPipes()
    {
        if (MustBeStraight && MustBeTurn) MustBeStraight = false;

        if (MustBeTurn || MustBeCross)
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                    PipeRight = true;
                    PipeUp = true;
                    break;

                case 1:
                    PipeLeft = true;
                    PipeUp = true;
                    break;

                case 2:
                    PipeRight = true;
                    PipeDown = true;
                    break;

                case 3:
                    PipeLeft = true;
                    PipeDown = true;
                    break;
            }
        }
        
        if (MustBeStraight || MustBeCross)
        {
            switch (Random.Range(0, 2))
            {
                case 0:
                    PipeRight = true;
                    PipeLeft = true;
                    break;

                case 1:
                    PipeDown = true;
                    PipeUp = true;
                    break;
            }
        }
        
        
        if (Random.Range(0f, 1f) < RandomPipeChance) PipeRight = true;
        if (Random.Range(0f, 1f) < RandomPipeChance) PipeUp = true;
        if (Random.Range(0f, 1f) < RandomPipeChance) PipeLeft = true;
        if (Random.Range(0f, 1f) < RandomPipeChance) PipeDown = true;
        
        if (puzzle.EndTile == this || puzzle.EndTile2 == this)
        {
            PipeLeft = true;
            PipeDown = false;
            PipeUp = false;
            PipeRight = false;
            Image.color = Color.red;
        }

        if (puzzle.StartTile == this)
        {
            PipeLeft = true;
            PipeRight = false;
            PipeDown = false; 
            PipeUp = false;
        }
    }


    /// <summary>
    /// Sets this tile's sprite and rotation based on which pipes the tile has active to start
    /// </summary>
    public void SetSprite()
    {
        int spriteIndex = 0;
        int numPipes = (PipeRight ? 1 : 0) + (PipeUp ? 1 : 0) + (PipeLeft ? 1 : 0) + (PipeDown ? 1 : 0);
        int rotation = 0;
        switch (numPipes)
        {
            case 0:
                break;

            case 1:
                spriteIndex = 1;
                if (PipeUp) rotation = 90;
                else if (PipeLeft) rotation = 180;
                else if (PipeDown) rotation = 270;
                break;

            case 2:
                if ((PipeRight && PipeLeft) || (PipeUp && PipeDown)) spriteIndex = 3;
                else spriteIndex = 2;

                if (PipeUp && PipeLeft) rotation = 90;
                else if (PipeLeft && PipeDown) rotation = 180;
                else if (PipeDown && PipeRight) rotation = 270;
                else if (PipeUp && PipeDown) rotation = 90;
                break;

            case 3:
                spriteIndex = 4;
                if (!PipeDown) rotation = 90;
                else if (!PipeRight) rotation = 180;
                else if (!PipeUp) rotation = 270;
                break;

            case 4:
                spriteIndex = 5;
                break;
        }
        if (spriteIndex == 0)
        {
            Image.enabled = false;
            return;
        }
        Image.sprite = puzzle.TileSprites[spriteIndex - 1]; // as of 5/7/25, there's no empty sprite
        Image.transform.eulerAngles += Vector3.forward * (transform.eulerAngles.z + rotation);
    }

    /// <summary>
    /// Rotates this tile 90 degrees clockwise, updating its sprite and pipe states.
    /// </summary>
    public IEnumerator RotateThisTile()
    {
        animating = true;
        float startRot = Image.transform.eulerAngles.z;
        float totalRot = 0f;
        float width = puzzle.TileWidth;


        do
        {
            yield return new WaitForSeconds(popSpeed);
            float rot = popRotSpeed * (popEasingScale + 1 - Mathf.Pow((totalRot / 45f) - 1f, 2)) / popEasingScale;
            Image.transform.localScale = Vector3.one * (1 + popSizeScale *  rot / popRotSpeed);
            Image.transform.eulerAngles -= Vector3.forward * rot;
            totalRot += rot;
        } while (totalRot <= 90f);
        Image.transform.localScale = Vector3.one;
        Image.transform.eulerAngles = Vector3.forward * (startRot - 90f);
        animating = false;

        bool temp = PipeRight;
        PipeRight = PipeUp;
        PipeUp = PipeLeft;
        PipeLeft = PipeDown;
        PipeDown = temp;
        puzzle.ResetPuzzle();
    }

    /// <summary>
    /// Fills the four tiles touching this tile with water if they have valid pipe connections
    /// </summary>
    public void FillAdjacentTiles()
    {
        if (!HasWater) return;
        WaterPuzzleTile[] adjTiles = GetAdjacentTiles();
        // right
        if (adjTiles[0] != null) adjTiles[0].FillTile(PipeRight && adjTiles[0].PipeLeft);
        // up
        if (adjTiles[1] != null) adjTiles[1].FillTile(PipeUp && adjTiles[1].PipeDown);
        // left
        if (adjTiles[2] != null) adjTiles[2].FillTile(PipeLeft && adjTiles[2].PipeRight);
        // down
        if (adjTiles[3] != null) adjTiles[3].FillTile(PipeDown && adjTiles[3].PipeUp);
    }

    /// <summary>
    /// The four tiles adjacent to this tile, in the order of right, up, left, then down
    /// </summary>
    /// <returns>Array with the WaterPuzzleTile component of those tiles</returns>
    public WaterPuzzleTile[] GetAdjacentTiles()
    {
        WaterPuzzleTile[] result = new WaterPuzzleTile[4];

        // right
        if (PosX < puzzle.GridWidth - 1)
        {
            result[0] = puzzle.Tiles[PosX + 1, PosY];
        }
        // up
        if (PosY > 0)
        {
            result[1] = puzzle.Tiles[PosX, PosY - 1];
        }
        // left
        if (PosX > 0)
        {
            result[2] = puzzle.Tiles[PosX - 1, PosY];
        }
        // down
        if (PosY < puzzle.GridHeight - 1)
        {
            result[3] = puzzle.Tiles[PosX, PosY + 1];
        }

        return result;
    }

    /// <summary>
    /// Fill the tile with water, and update its sprite accordingly
    /// For now, this just makes the tile blue
    /// </summary>
    /// <param name="filled"> Whether or not the tile should actually be filled</param>
    public void FillTile(bool filled)
    {
        if (filled && !HasWater)
        {
            Image.color = fillColor;
            HasWater = true;
            FillAdjacentTiles();

            if (puzzle.EndTile.HasWater && (!puzzle.twoEndings || puzzle.EndTile2.HasWater) && !puzzle.IsComplete)
            {
                StartCoroutine(puzzle.CompletePuzzle());
            }


        }
        
    }

    /// <summary>
    /// Get rid of the water in this tile
    /// </summary>
    public void EmptyTile()
    {
        if (puzzle.EndTile != this && puzzle.EndTile2 != this) Image.color = Color.white;
        else Image.color = Color.red;
        HasWater = false;
    }

    public void UsePipeSplitterOnTile()
    {
        if (!puzzle.twoEndings) return;
        puzzle.SplitTile = this;
        puzzle.SplitTilePipes[0] = PipeRight;
        puzzle.SplitTilePipes[1] = PipeUp;
        puzzle.SplitTilePipes[2] = PipeLeft;
        puzzle.SplitTilePipes[3] = PipeDown;
        PipeRight = PipeUp = PipeLeft = PipeDown = true;
        SetSprite();
        puzzle.UsedPipeSplitter = true;
        puzzle.ToggleSplitTile();
        puzzle.ResetPuzzle();
    }

    public void OnClick()
    {
        if (!puzzle.IsComplete && !animating)
        {
            if (!puzzle.UsedPipeSplitter && puzzle.PipeSplitterToggled) UsePipeSplitterOnTile();
            else StartCoroutine(RotateThisTile());
        }
    }
}
