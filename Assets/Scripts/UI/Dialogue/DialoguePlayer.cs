using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DialoguePlayer : MonoBehaviour
{
    // object references
    public GameObject DialogueWindow;
    [SerializeField] private Image textboxRect;
    [SerializeField] private Sprite mariRect;
    [SerializeField] private Sprite unnRect;
    [SerializeField] private Sprite mariRadio;
    [SerializeField] private Sprite unnRadio;

    [SerializeField] private TMP_Text speakerTarget;
    [SerializeField] private TMP_Text lineTarget;

    [SerializeField] private Image frame;
    [SerializeField] private Sprite mariFrame;
    [SerializeField] private Sprite unnFrame;

    [SerializeField] private Image mask;
    [SerializeField] private Image portrait;

    // dialogue control
    private List<DialogueElement> conversation;
    private int dialogueIndex;

    // typewriter control
    private bool finishedTypewriter;
    private const float DIALOGUE_SPEED = 0.025f;

    // Tag removal
    private Regex tagPattern = new Regex(@"<[^>]*>"); // Matches rich text tags like <i>text</i>
    private string taglessText = "";


    void Start()
    {
        DialogueWindow.SetActive(false);
    }

    public void PlayDialogue(List<DialogueElement> dialogue)
    {
        conversation = dialogue;
        dialogueIndex = -1;

        DialogueWindow.SetActive(true);
        if(PlayerController.Instance) PlayerController.Instance.SetMovementLock(true);

        // check if Mariposa currently active
        if (!PlayerController.Instance || Player.ActivePlayer.Data.characterID == CharID.Mariposa)
        {
            frame.sprite = mariFrame;
            textboxRect.sprite = mariRect;
        }
        else
        {
            frame.sprite = unnFrame;
            textboxRect.sprite = unnRect;
        }

        AdvanceDialogue();
    }

    public void TryAdvanceDialogue()
    {
        // if typewriter effect not finished yet
        if (!finishedTypewriter)
        {
            // finish typewriter effect
            StopAllCoroutines();
            finishedTypewriter = true;
            lineTarget.maxVisibleCharacters = taglessText.Length;
        }
        else
        {
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        dialogueIndex++;

        // check if conversation ended
        if (dialogueIndex >= conversation.Count)
        {
            DialogueWindow.SetActive(false);
            if(PlayerController.Instance) PlayerController.Instance.SetMovementLock(false);
            return;
        }
        
        // start dialogue
        taglessText = tagPattern.Replace(conversation[dialogueIndex].Line, string.Empty);
        speakerTarget.text = conversation[dialogueIndex].Speaker;
        lineTarget.text = conversation[dialogueIndex].Line;
        StartCoroutine(TypewriterEffect());
    }

    
    private IEnumerator TypewriterEffect()
    {
        finishedTypewriter = false;
        int length = taglessText.Length;
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
