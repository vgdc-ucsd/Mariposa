using UnityEngine;

public class DialogueCranePressurePlate : CranePressurePlate
{
    [SerializeField] private string notEnoughBatteriesDialogue;
    [SerializeField] private string enoughBatteriesDialogue;
    [SerializeField] private GameObject destroyedTerrain;
    [SerializeField] private BoxCollider2D ghostCollider;
    [SerializeField] private GameObject grappleToHide;

    public override void NotEnoughBatteries()
    {
        DialogueManager.Instance.PlayDialogue(notEnoughBatteriesDialogue);
    }

    public override void EnoughBatteries()
    {
        destroyedTerrain.SetActive(false);
        DialogueManager.Instance.PlayDialogue(enoughBatteriesDialogue);
        ghostCollider.gameObject.SetActive(false);
        grappleToHide.SetActive(false);
    }
}
