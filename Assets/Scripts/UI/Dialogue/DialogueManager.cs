using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

public class DialogueManager : Singleton<DialogueManager>
{
    // object references
    public GameObject DialogueWindow;
    [SerializeField] Image rect;
    [SerializeField] Sprite mariRect;
    [SerializeField] Sprite unnRect;
    [SerializeField] Sprite mariRadio;
    [SerializeField] Sprite unnRadio;

    [SerializeField] TMP_Text speakerTarget;
    [SerializeField] TMP_Text lineTarget;

    [SerializeField] GameObject frame;
    [SerializeField] Sprite mariFrame;
    [SerializeField] Sprite unnFrame;

    [SerializeField] Image mask;
    [SerializeField] Image portrait;

    // dialogue control
    List<DialogueElement> conversation;
    int dialogueIndex;

    // typewriter control
    [SerializeField] float DIALOGUE_SPEED = 0.025f;
    public bool finishedTypewriter;

    void Start()
    {
        DialogueWindow.SetActive(false);
    }

    public void PlayDialogue(Dialogue dialogue)
    {
        Debug.Log("Started dialogue");
        
        conversation = dialogue.Conversation;
        dialogueIndex = -1;

        DialogueWindow.SetActive(true);
        AdvanceDialogue();
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
            speakerTarget.text = conversation[dialogueIndex].Speaker;
            lineTarget.text = conversation[dialogueIndex].Line;
            StartCoroutine(TypewriterEffect());

            // check if Mariposa currently active
            if (Player.ActivePlayer.Character.Id == CharID.Mariposa)
            //if(true)    //placeholder for testing before merge
            {
                frame.GetComponent<Image>().sprite = mariFrame;

                // check if from radio
                if (conversation[dialogueIndex].FromRadio)
                {
                    rect.sprite = mariRadio;
                }
                else
                {
                    rect.sprite = mariRect;
                }
            }
            else
            {
                frame.GetComponent<Image>().sprite = unnFrame;

                // check if from radio
                // check if from radio
                if (conversation[dialogueIndex].FromRadio)
                {
                    rect.sprite = unnRadio;
                }
                else
                {
                    rect.sprite = unnRect;
                }
            }

            // check if has sprite
            if (conversation[dialogueIndex].Sprite != null)
            {
                // resize text boxes
                speakerTarget.GetComponent<RectTransform>().offsetMin = new Vector2(150, -60);
                lineTarget.GetComponent<RectTransform>().offsetMin = new Vector2(150, -80);
                // show sprite
                portrait.sprite = conversation[dialogueIndex].Sprite;
                Debug.Log("Set sprite to " +  conversation[dialogueIndex].Sprite);
                frame.SetActive(true);
            }
            else
            {
                // resize text boxes
                speakerTarget.GetComponent<RectTransform>().offsetMin = new Vector2(45, -60);
                lineTarget.GetComponent<RectTransform>().offsetMin = new Vector2(45, -80);
                // hide sprite
                frame.SetActive(false);
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
                lineTarget.maxVisibleCharacters = conversation[dialogueIndex].Line.Length;
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
        lineTarget.maxVisibleCharacters = i;
        while (i < length)
        {
            float elapsedTime = Time.time - startTime;
            if (elapsedTime > DIALOGUE_SPEED)
            {
                i++;
                lineTarget.maxVisibleCharacters = i;
                startTime = Time.time;
            }
            else yield return null;
        }

        finishedTypewriter = true;
    }
}
