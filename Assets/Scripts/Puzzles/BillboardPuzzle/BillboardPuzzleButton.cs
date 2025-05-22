using UnityEngine;

public class BillboardPuzzleButton : MonoBehaviour
{
    [SerializeField] private bool beeMode;
    [SerializeField] private int row, col;


    private new void Start()
    {
        
    }


    

    public void OnClick()
    {
        if (beeMode)
        {

            BillboardPuzzle.Instance.ShiftRow(row);

        }
        else
        {
            BillboardPuzzle.Instance.RotateCol(col);

        }
    }


    


}
