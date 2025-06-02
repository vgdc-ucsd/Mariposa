using UnityEngine;

public class BillboardPuzzleButton : MonoBehaviour
{
    [SerializeField] private bool beeMode;
    [SerializeField] private int row, col;
    [SerializeField] private float rotScale;

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
            transform.eulerAngles += (col % 2 == 0 ? -1 : 1) * rotScale * Vector3.forward;
        }
    }


    


}
