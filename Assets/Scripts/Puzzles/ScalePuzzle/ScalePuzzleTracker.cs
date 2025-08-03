using UnityEngine;

public class ScalePuzzleTracker : MonoBehaviour
{
    [SerializeField] private PuzzleInteractable level1Trigger;
    [SerializeField] private PuzzleInteractable level2Trigger;
    [SerializeField] private PuzzleInteractable level3Trigger;
    [SerializeField] private string finishLevel1Dialogue;
    [SerializeField] private string finishLevel2Dialogue;
    [SerializeField] private string finishLevel3Dialogue;

    public void CompleteLevel1()
    {
        level1Trigger.gameObject.SetActive(false);
        level2Trigger.gameObject.SetActive(true);
        DialogueManager.Instance.PlayDialogue(finishLevel1Dialogue);
    }

    public void CompleteLevel2()
    {
        level2Trigger.gameObject.SetActive(false);
        level3Trigger.gameObject.SetActive(true);
        DialogueManager.Instance.PlayDialogue(finishLevel2Dialogue);
    }

    public void CompleteLevel3()
    {
        level3Trigger.gameObject.SetActive(false);
        DialogueManager.Instance.PlayDialogue(finishLevel3Dialogue);
    }
}
