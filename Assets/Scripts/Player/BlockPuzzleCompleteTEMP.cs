using UnityEngine;

public class BlockPuzzleCompleteTEMP : MonoBehaviour
{
    public Dialogue dialogue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DialogueManager.Instance.PlayDialogue(dialogue, () =>
        {
            LevelManager.Instance.GoToNextSublevel();
        });
    }
}
