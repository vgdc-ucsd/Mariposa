using UnityEngine;


/// <summary>
/// Used for areas where the player is prevented from going a direction, and dialgoue plays telling the player to turn around
/// </summary>
public class TutorialDialogueWall : Trigger
{
    [SerializeField] private GameObject snapBackPoint;
    public string DialogueName;
    protected override bool MustBePlayer => true;
    protected override bool OnlyOnce => false;
    public override bool OnEnter(Body body)
    {
        if (!base.OnEnter(body)) return false;
        body.transform.position = snapBackPoint.transform.position;
        DialogueManager.Instance.PlayDialogue(DialogueName);
        return true;
    }
}
