using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

// dialogue model
public class DialogueManager : Singleton<DialogueManager>
{
    public GameObject DialogueWindow;
    [SerializeField] TMP_Text SpeakerTarget;
    [SerializeField] TMP_Text LineTarget;
    [SerializeField] GameObject Frame;
    [SerializeField] Image Mask;
    [SerializeField] Image Portrait;

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

            // check if has sprite
            if (conversation[dialogueIndex].Sprite != null)
            {
                // resize text boxes
                SpeakerTarget.GetComponent<RectTransform>().offsetMin = new Vector2(150, -60);
                LineTarget.GetComponent<RectTransform>().offsetMin = new Vector2(150, -80);
                // show sprite
                Portrait.sprite = conversation[dialogueIndex].Sprite;
                Frame.SetActive(true);
            }
            else
            {
                // resize text boxes
                SpeakerTarget.GetComponent<RectTransform>().offsetMin = new Vector2(45, -60);
                LineTarget.GetComponent<RectTransform>().offsetMin = new Vector2(45, -80);
                // hide sprite
                Frame.SetActive(false);
            }
        }
    }
}