using UnityEngine;

public class WaterPuzzleTile : MonoBehaviour
{
    public bool pipeRight, pipeUp, pipeLeft, pipeDown;
    public bool hasWater;
    public int posX, posY;
    public BoxCollider2D myCollider;
    public SpriteRenderer spriteRenderer;


    private void Awake()
    {
        
        EmptyTile();

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WaterPuzzle.Instance.tiles[posX, posY] = this;
        if (WaterPuzzle.Instance.startTile == this) FillTile(true);
        else EmptyTile();

        if (WaterPuzzle.Instance.endTile == this) spriteRenderer.material.color = Color.red;
        SetSprite();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        if (Input.GetMouseButtonDown(0) && !WaterPuzzle.Instance.puzzleComplete && myCollider.bounds.Contains(mousePos))
        {
            RotateThisTile();
            WaterPuzzle.Instance.ResetPuzzle();
        }
    }

    /// <summary>
    /// Sets this tile's sprite and rotation based on which pipes the tile has active to start
    /// </summary>
    public void SetSprite()
    {
        int spriteIndex = 0;
        int numPipes = (pipeRight ? 1 : 0) + (pipeUp ? 1 : 0) + (pipeLeft ? 1 : 0) + (pipeDown ? 1 : 0);
        int rotation = 0;
        switch (numPipes)
        {
            case 0:
                break;

            case 1:
                spriteIndex = 1;
                if (pipeUp) rotation = 90;
                else if (pipeLeft) rotation = 180;
                else if (pipeDown) rotation = 270;
                break;

            case 2:
                if ((pipeRight && pipeLeft) || (pipeUp && pipeDown)) spriteIndex = 3;
                else spriteIndex = 2;

                if (pipeUp && pipeLeft) rotation = 90;
                else if (pipeLeft && pipeDown) rotation = 180;
                else if (pipeDown && pipeRight) rotation = 270;
                else if (pipeUp && pipeDown) rotation = 90;
                break;

            case 3:
                spriteIndex = 4;
                if (!pipeDown) rotation = 90;
                else if (!pipeRight) rotation = 180;
                else if (!pipeUp) rotation = 270;
                break;

            case 4:
                spriteIndex = 5;
                break;
        }

        spriteRenderer.sprite = WaterPuzzle.Instance.tileSprites[spriteIndex];
        transform.eulerAngles += Vector3.forward * (transform.eulerAngles.z + rotation);
    }

    /// <summary>
    /// Rotates this tile 90 degrees clockwise, updating its sprite and pipe states.
    /// </summary>
    public void RotateThisTile()
    {
        bool temp = pipeRight;
        pipeRight = pipeUp;
        pipeUp = pipeLeft;
        pipeLeft = pipeDown;
        pipeDown = temp;
        transform.eulerAngles -= Vector3.forward * 90;
    }

    /// <summary>
    /// Fills the four tiles touching this tile with water if they have valid pipe connections
    /// </summary>
    public void FillAdjacentTiles()
    {
        if (!hasWater) return;
        WaterPuzzleTile[] adjTiles = GetAdjacentTiles();
        // right
        if (adjTiles[0] != null) adjTiles[0].FillTile(pipeRight && adjTiles[0].pipeLeft);
        // up
        if (adjTiles[1] != null) adjTiles[1].FillTile(pipeUp && adjTiles[1].pipeDown);
        // left
        if (adjTiles[2] != null) adjTiles[2].FillTile(pipeLeft && adjTiles[2].pipeRight);
        // down
        if (adjTiles[3] != null) adjTiles[3].FillTile(pipeDown && adjTiles[3].pipeUp);
    }

    /// <summary>
    /// The four tiles adjacent to this tile, in the order of right, up, left, then down
    /// </summary>
    /// <returns>Array with the WaterPuzzleTile component of those tiles</returns>
    public WaterPuzzleTile[] GetAdjacentTiles()
    {
        WaterPuzzleTile[] result = new WaterPuzzleTile[4];

        // right
        if (posX < WaterPuzzle.Instance.gridWidth - 1)
        {
            result[0] = WaterPuzzle.Instance.tiles[posX + 1, posY];
        }
        // up
        if (posY > 0)
        {
            result[1] = WaterPuzzle.Instance.tiles[posX, posY - 1];
        }
        // left
        if (posX > 0)
        {
            result[2] = WaterPuzzle.Instance.tiles[posX - 1, posY];
        }
        // down
        if (posY < WaterPuzzle.Instance.gridHeight - 1)
        {
            result[3] = WaterPuzzle.Instance.tiles[posX, posY + 1];
        }

        return result;
    }

    /// <summary>
    /// Fill the tile with water, and update its sprite accordingly
    /// For now, this just makes the tile blue
    /// </summary>
    public void FillTile(bool filled)
    {
        if (filled && !hasWater)
        {
            if (WaterPuzzle.Instance.endTile == this)
            {
                WaterPuzzle.Instance.CompletePuzzle();
            }

            spriteRenderer.material.color = Color.blue;
            hasWater = true;
            FillAdjacentTiles();
        }
    }

    public void EmptyTile()
    {
        if (WaterPuzzle.Instance.endTile != this) spriteRenderer.material.color = Color.white;
        hasWater = false;
    }

    
}
