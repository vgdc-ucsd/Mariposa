using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using Unity.Properties;

public class BillboardPuzzleInteractable : GenericInteractable
{
    [SerializeField] private BillboardPuzzle puzzle;

    private void Start()
    {
        if (!puzzle.Initialized) puzzle.Initialize();
    }

    public override void OnInteract(IControllable controllable)
    {
        Debug.Log("let him cook");
        PuzzlePopupManager.Instance.ActivePuzzle = puzzle.gameObject;
        base.OnInteract(controllable);
    }

    
    private void Update()
    {
        if (isActiveAndEnabled == puzzle.isActiveAndEnabled) gameObject.SetActive(!puzzle.isActiveAndEnabled);
    }
}
