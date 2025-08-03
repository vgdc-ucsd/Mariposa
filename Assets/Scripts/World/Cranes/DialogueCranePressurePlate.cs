using UnityEngine;

public class DialogueCranePressurePlate : CranePressurePlate
{
    [SerializeField] private string notEnoughBatteriesDialogue;
    [SerializeField] private string enoughBatteriesDialogue;
    [SerializeField] private GameObject destroyedTerrain;
    [SerializeField] private BoxCollider2D ghostCollider;

    public override void NotEnoughBatteries()
    {
        DialogueManager.Instance.PlayDialogue(notEnoughBatteriesDialogue);
    }

    public override void EnoughBatteries()
    {
        destroyedTerrain.SetActive(false);
        DialogueManager.Instance.PlayDialogue(enoughBatteriesDialogue);
        ghostCollider.gameObject.SetActive(false);
    }
}
