using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaterPuzzleTile : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public bool PipeRight, PipeUp, PipeLeft, PipeDown;
    [HideInInspector] public bool HasWater;
    [HideInInspector] public int PosX, PosY;
    [HideInInspector] public BoxCollider2D MyCollider;
    [HideInInspector] public Image Image;
    [HideInInspector] public bool MustBeTurn;
    [HideInInspector] public bool MustBeStraight;
    [HideInInspector] public bool MustBeCross;

    public float RandomPipeChance; // odds from 0 to 1 per side on this tile for a pipe to randomly spawn there

   
    private void Start()
    {
        if (WaterPuzzle.Instance.StartTile == this) FillTile(true);
        else EmptyTile();

        if (WaterPuzzle.Instance.EndTile == this || WaterPuzzle.Instance.EndTile2 == this) Image.color = Color.red;

        SetPipes();
        SetSprite();
    }

    /// <summary>
    /// Called right when this tile is created to initialize key variables.
    /// </summary>
    public void InitializeTile()
    {
        MyCollider = GetComponent<BoxCollider2D>();
        Image = GetComponent<Image>();
    }

    /// <summary>
    /// Assigns values to PipeRight, PipeUp, PipeLeft, and PipeDown, making sure that this tile can connect to adjacent tiles if it's part of the solution.
    /// </summary>
    private void SetPipes()
    {
        

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
        //if (MustBeCross) PipeRight = PipeUp = PipeLeft = PipeDown = true;
        
        if (!(MustBeTurn || MustBeStraight || MustBeCross))
        {
            if (Random.Range(0f, 1f) < RandomPipeChance) PipeRight = true;
            if (Random.Range(0f, 1f) < RandomPipeChance) PipeUp = true;
            if (Random.Range(0f, 1f) < RandomPipeChance) PipeLeft = true;
            if (Random.Range(0f, 1f) < RandomPipeChance) PipeDown = true;
        }
        if (WaterPuzzle.Instance.EndTile == this || WaterPuzzle.Instance.EndTile2 == this)
        {
            PipeLeft = true;
            PipeDown = false;
            PipeUp = false;
            PipeRight = false;
            Image.color = Color.red;
        }

        if (WaterPuzzle.Instance.StartTile == this)
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

        Image.sprite = WaterPuzzle.Instance.TileSprites[spriteIndex];
        transform.eulerAngles += Vector3.forward * (transform.eulerAngles.z + rotation);
    }

    /// <summary>
    /// Rotates this tile 90 degrees clockwise, updating its sprite and pipe states.
    /// </summary>
    public void RotateThisTile()
    {
        bool temp = PipeRight;
        PipeRight = PipeUp;
        PipeUp = PipeLeft;
        PipeLeft = PipeDown;
        PipeDown = temp;
        transform.eulerAngles -= Vector3.forward * 90;
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
        if (PosX < WaterPuzzle.Instance.GridWidth - 1)
        {
            result[0] = WaterPuzzle.Instance.Tiles[PosX + 1, PosY];
        }
        // up
        if (PosY > 0)
        {
            result[1] = WaterPuzzle.Instance.Tiles[PosX, PosY - 1];
        }
        // left
        if (PosX > 0)
        {
            result[2] = WaterPuzzle.Instance.Tiles[PosX - 1, PosY];
        }
        // down
        if (PosY < WaterPuzzle.Instance.GridHeight - 1)
        {
            result[3] = WaterPuzzle.Instance.Tiles[PosX, PosY + 1];
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
        Debug.Log(filled);
        if (filled && !HasWater)
        {
            /*if (WaterPuzzle.Instance.EndTile.HasWater && (!WaterPuzzle.Instance.twoEndings || WaterPuzzle.Instance.EndTile2.HasWater))
            {
                Debug.Log("should complete puzzle");
                WaterPuzzle.Instance.CompletePuzzle();
            }*/

            Image.color = Color.blue;
            HasWater = true;
            FillAdjacentTiles();
        }
        
    }

    /// <summary>
    /// Get rid of the water in this tile
    /// </summary>
    public void EmptyTile()
    {
        if (WaterPuzzle.Instance.EndTile != this && WaterPuzzle.Instance.EndTile2 != this) Image.color = Color.white;
        HasWater = false;
    }

    public void UsePipeSplitterOnTile()
    {
        if (!WaterPuzzle.Instance.twoEndings) return; 
        PipeRight = PipeUp = PipeLeft = PipeDown = true;
        SetSprite();
        WaterPuzzle.Instance.UsedPipeSplitter = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!WaterPuzzle.Instance.PuzzleComplete)
        {
            if (Input.GetKey(KeyCode.LeftControl) && !WaterPuzzle.Instance.UsedPipeSplitter) UsePipeSplitterOnTile();
            RotateThisTile();
            WaterPuzzle.Instance.ResetPuzzle();
        }
    }
}
