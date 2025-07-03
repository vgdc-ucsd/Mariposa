using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialJammedDoor : Interactable
{
    [SerializeField] private string jammedDialogue;
    public bool jammed, doneWithFirstDialogue;

    public override void OnInteract(IControllable controllable)
    {
        if (jammed && !doneWithFirstDialogue)
        {
            DialogueManager.Instance.PlayDialogue(jammedDialogue);
            doneWithFirstDialogue = true;
        }

        if (!jammed)
        {
            SceneManager.LoadScene(2);
        }

    }
}
