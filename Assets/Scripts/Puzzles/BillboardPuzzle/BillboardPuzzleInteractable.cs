using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using Unity.Properties;

public class BillboardPuzzleInteractable : Interactable
{
    [SerializeField] private BillboardPuzzle puzzle;
    private BoxCollider2D playerCollider;
    private BoxCollider2D myCollider;
    public override void OnInteract()
    {
    }

    protected override void SetProximity()
    {
    }

    private void Start()
    {
        if (Player.ActivePlayer) playerCollider = Player.ActivePlayer.GetComponent<BoxCollider2D>();
        myCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (isActiveAndEnabled == puzzle.isActiveAndEnabled) gameObject.SetActive(!puzzle.isActiveAndEnabled);
        if (!Input.GetKeyDown(KeyCode.E)) return;
        if (playerCollider && myCollider.bounds.Contains(playerCollider.bounds.center))
        {
            PuzzlePopupManager.Instance.ActivePuzzle = puzzle.gameObject;
        }
    }
}
