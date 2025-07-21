using UnityEngine;

public class DialogueCranePressurePlate : CranePressurePlate
{
    [SerializeField] private string notEnoughBatteriesDialogue;
    [SerializeField] private string enoughBatteriesDialogue;
    public override void NotEnoughBatteries()
    {
        DialogueManager.Instance.PlayDialogue(notEnoughBatteriesDialogue);
    }

    public override void EnoughBatteries()
    {
        DialogueManager.Instance.PlayDialogue(enoughBatteriesDialogue);
    }
}
