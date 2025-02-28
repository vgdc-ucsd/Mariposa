using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : Singleton<DialogueManager>
{
    // object references
    public GameObject DialogueWindow;
    [SerializeField] TMP_Text SpeakerTarget;
    [SerializeField] TMP_Text LineTarget;
    [SerializeField] GameObject Frame;
    [SerializeField] Image Mask;
    [SerializeField] Image Portrait;

    // dialogue control
    List<DialogueElement> conversation;
    int dialogueIndex;

    // typewriter control
    const float DIALOGUE_SPEED = 0.025f;
    bool finishedTypewriter;

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

        // check if conversation ended
        if (dialogueIndex >= conversation.Count)
        {
            DialogueWindow.SetActive(false);
            Debug.Log("Ended Dialogue");
        }
        else
        {
            // start dialogue
            SpeakerTarget.text = conversation[dialogueIndex].Speaker;
            LineTarget.text = conversation[dialogueIndex].Line;
            StartCoroutine(TypewriterEffect());

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

    public void TryAdvanceDialogue()
    {
        // if typewriter effect not finished yet
        if (!finishedTypewriter)
            {
                // finish typewriter effect
                StopAllCoroutines();
                finishedTypewriter = true;
                LineTarget.maxVisibleCharacters = conversation[dialogueIndex].Line.Length;
            }
        else
            {
                AdvanceDialogue();
            }
    }

    private IEnumerator TypewriterEffect()
    {
        finishedTypewriter = false;
        int length = conversation[dialogueIndex].Line.Length;
        float startTime = Time.time;

        int i = 0;
        LineTarget.maxVisibleCharacters = i;
        while (i < length)
        {
            float elapsedTime = Time.time - startTime;
            if (elapsedTime > DIALOGUE_SPEED)
            {
                i++;
                LineTarget.maxVisibleCharacters = i;
                startTime = Time.time;
            }
            else yield return null;
        }

        finishedTypewriter = true;
    }
}