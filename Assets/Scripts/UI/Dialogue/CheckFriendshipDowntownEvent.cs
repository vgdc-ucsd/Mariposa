using UnityEngine;

public class CheckFriendshipDowntownEvent : DialogueEvent
{
    public override void Trigger()
    {
        if (FriendshipManager.Instance.CompareScore(5))
        {
            DialogueManager.Instance.PlayDialogue("ocean_friends");
        }
        else
        {
            DialogueManager.Instance.PlayDialogue("no_ocean_friends");
        }
    }
}
