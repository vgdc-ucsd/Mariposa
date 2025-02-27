using UnityEngine;

public class BillboardPuzzleButton : Interactable
{
    [SerializeField] private bool beeMode;
    private GameObject bee;
    private BeeControlAbility beeAbility;
    private GameObject player;
    private BoxCollider2D myCollider, beeCollider, playerCollider;
    [SerializeField] private int row, col;

    public override void OnInteract()
    {
        throw new System.NotImplementedException();
    }

    protected override void SetProximity()
    {
        throw new System.NotImplementedException();
    }

    private new void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
        player = Player.ActivePlayer.gameObject;
        if (Player.ActivePlayer.Ability is BeeControlAbility)
        {
            beeAbility = (BeeControlAbility)Player.ActivePlayer.Ability;
            bee = (beeAbility).BeeRef.gameObject;
            beeCollider = bee.GetComponent<BoxCollider2D>();
            playerCollider = player.GetComponent<BoxCollider2D>();
        }
    }

    void Update()
    {
        // Uncomment these next few lines if you want the bee to follow the mouse instead of moving how it normally does.
        /*if (bee != null)
        {
            bee.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bee.transform.position -= Vector3.forward * bee.transform.position.z;
        }*/
        // temp input reading until entire input system is ready
        if (!Input.GetKeyDown(KeyCode.E)) return;

        if (beeMode && beeAbility.controllingBee)
        {
            if (myCollider.bounds.Contains(beeCollider.bounds.center))
            {
                BillboardPuzzle.Instance.ShiftRow(row);
            }
        }
        else if (!beeAbility.controllingBee)
        {
            if (myCollider.bounds.Contains(playerCollider.bounds.center))
            {
                BillboardPuzzle.Instance.RotateCol(col);
            }
        }
    }

    
}
