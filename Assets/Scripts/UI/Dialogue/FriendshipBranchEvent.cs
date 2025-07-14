using UnityEngine;

public class FriendshipBranchEvent : DialogueEvent
{
    [SerializeField] private int friendshipThreshold;
    [SerializeField] private string goodBranch;
    [SerializeField] private string badBranch;

    public override void Trigger()
    {
        if (FriendshipManager.Instance.CompareScore(friendshipThreshold)) DialogueManager.Instance.PlayDialogue(goodBranch);
        else DialogueManager.Instance.PlayDialogue(badBranch);
    }
}
