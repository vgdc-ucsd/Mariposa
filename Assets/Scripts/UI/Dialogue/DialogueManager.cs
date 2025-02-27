using UnityEngine;
using System.Collections.Generic;
using TMPro;

// dialogue model
public class DialogueManager : Singleton<DialogueManager>
{
    public GameObject DialogueWindow;
    [SerializeField] TMP_Text SpeakerTarget;
    [SerializeField] TMP_Text LineTarget;

    List<DialogueElement> conversation;
    int dialogueIndex;


    void Start()
    {
        DialogueWindow.SetActive(false);
    }

    public void PlayDialogue(Dialogue dialogue)
    {
        conversation = dialogue.Conversation;
        dialogueIndex = -1;

        DialogueWindow.SetActive(true);
        AdvanceDialogue();

        Debug.Log("Started dialogue");
    }

    public void AdvanceDialogue()
    {
        dialogueIndex++;

        Debug.Log("dialogueIndex = " + dialogueIndex);

        // check if conversation ended
        if (dialogueIndex >= conversation.Count)
        {
            DialogueWindow.SetActive(false);
            Debug.Log("Ended Dialogue");
        }
        else
        {
            SpeakerTarget.text = conversation[dialogueIndex].Speaker;
            LineTarget.text = conversation[dialogueIndex].Line;
        }
    }
}