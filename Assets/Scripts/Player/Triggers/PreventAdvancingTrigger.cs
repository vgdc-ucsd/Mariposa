using System.Collections;
using UnityEngine;

public class PreventAdvancingTrigger : Trigger
{
    [SerializeField] private ItemData requiredItem;
    [SerializeField] private Transform walkTo;
    [SerializeField] private string dialogue;

    public override bool OnEnter(Body body)
    {
        if (InventoryManager.Instance.GetInventory().HasItem(requiredItem))
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(MoveToTarget());
        }

        return base.OnEnter(body);
    }

    private IEnumerator MoveToTarget()
    {
        Vector2 playerPos = Player.ActivePlayer.transform.position;
        Vector2 moveDir = walkTo.position.x - playerPos.x < 0 ? Vector2.left : Vector2.right;
        PlayerController.Instance.SetMovementLock(true);

        while (Mathf.Abs(Player.ActivePlayer.transform.position.x - walkTo.position.x) > 0.5f)
        {
            Player.ActivePlayer.Movement.SetMoveDir(moveDir);
            yield return null;
        }

        Player.ActivePlayer.Movement.SetMoveDir(Vector2.zero);
        PlayerController.Instance.SetMovementLock(false);

        if (!string.IsNullOrEmpty(dialogue))
        {
            DialogueManager.Instance.PlayDialogue(dialogue);
        }
    }
}
