using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialJammedDoor : Interactable
{
    public Dialogue jammedDialogue, mariposaDialogue;
    [HideInInspector] public bool jammed = true;

    public override void OnInteract(IControllable controllable)
    {
        if (DialogueManager.Instance.IsPlayingDialogue || !TutorialManager.Instance.UnnamedHasRadio()) return;
        if (jammed)
        {
            DialogueManager.Instance.PlayDialogue(jammedDialogue, () =>
            {
                LevelManager.Instance.GoToNextSublevel();
                DialogueManager.Instance.PlayDialogue(mariposaDialogue);
            });
        }
        else
        {
            SceneManager.LoadScene(2); // city stage
        }
    }
}
