using Unity.VisualScripting;
using UnityEngine;

public class FinishWiresEvents : MonoBehaviour
{
    [SerializeField] private DialogueInteractableSimple wiresPuzzleOpenDialogue;
    [SerializeField] private GameObject billboardPuzzleInteractableObj;
    [SerializeField] private GameObject wiresPuzzleObj;
    [SerializeField] private GameObject billboardOffPanelObj;

    [SerializeField] private GameObject unlockedBillboardPuzzleInteractable;

    void Start()
    {
        unlockedBillboardPuzzleInteractable.SetActive(false);   
    }

    public void PlayCompleteWiresDialogue()
    {
        DialogueManager.Instance.PlayDialogue("wires_fixed");
        wiresPuzzleOpenDialogue.enabled = false;

        // wiresPuzzleObj.SetActive(false);
    }

    // activate billboard puzzle interactable and deactivate its previous dialogue prompt
    public void ActivateBillboardPuzzle()
    {
        billboardOffPanelObj.SetActive(false);
        unlockedBillboardPuzzleInteractable.SetActive(true);

        Transform defaultInteractionTrigger = billboardPuzzleInteractableObj.transform.Find("Interaction Trigger(Clone)");
        if (defaultInteractionTrigger != null ) defaultInteractionTrigger.gameObject.SetActive(false);
    }
}
