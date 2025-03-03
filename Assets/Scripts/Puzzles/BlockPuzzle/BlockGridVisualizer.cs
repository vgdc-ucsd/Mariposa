using UnityEngine;

public class BlockGridVisualizer : MonoBehaviour
{
    private BlockPuzzle puzzle;
    private LineRenderer[] horizontalLines;
    private LineRenderer[] verticalLines;

    void Start()
    {
        puzzle = BlockPuzzle.Instance;
        CreateGrid();
    }

    private void CreateGrid()
    {
        float startX = -puzzle.GridWidth / 2f;
        float startY = -puzzle.GridHeight / 2f;
        
        // Create horizontal lines
        horizontalLines = new LineRenderer[puzzle.GridHeight + 1];
        for (int y = 0; y <= puzzle.GridHeight; y++)
        {
            horizontalLines[y] = CreateLine($"HorizontalLine_{y}");
            SetLinePositions(horizontalLines[y], 
                new Vector3(startX * BlockPuzzle.Instance.GridScale, (startY + y) * BlockPuzzle.Instance.GridScale, 0), 
                new Vector3((startX + puzzle.GridWidth) * BlockPuzzle.Instance.GridScale, (startY + y) * BlockPuzzle.Instance.GridScale, 0));
        }

        // Create vertical lines
        verticalLines = new LineRenderer[puzzle.GridWidth + 1];
        for (int x = 0; x <= puzzle.GridWidth; x++)
        {
            verticalLines[x] = CreateLine($"VerticalLine_{x}");
            SetLinePositions(verticalLines[x], 
                new Vector3((startX + x) * BlockPuzzle.Instance.GridScale, startY * BlockPuzzle.Instance.GridScale, 0), 
                new Vector3((startX + x) * BlockPuzzle.Instance.GridScale, (startY + puzzle.GridHeight) * BlockPuzzle.Instance.GridScale, 0));
        }
    }

    private LineRenderer CreateLine(string name)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(transform);
        
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = line.endColor = new Color(1f, 1f, 1f, 0.5f);
        line.startWidth = line.endWidth = 0.05f;
        line.positionCount = 2;
        line.sortingOrder = -1;
        
        return line;
    }

    private void SetLinePositions(LineRenderer line, Vector3 start, Vector3 end)
    {
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }
}
