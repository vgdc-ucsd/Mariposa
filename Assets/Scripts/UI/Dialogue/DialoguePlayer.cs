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
    [SerializeField] private SpriteMap spriteMap;

    [SerializeField] private Image backgroundGraphic;
    [SerializeField] private SpriteMap backgroundMap;

    [SerializeField] private GameObject buttonDisplay;
    [SerializeField] private UnityEngine.UI.Button choiceButton1;
    [SerializeField] private UnityEngine.UI.Button choiceButton2;
    [SerializeField] private TextMeshProUGUI choiceText1;
    [SerializeField] private TextMeshProUGUI choiceText2;

    // dialogue control
    private List<DialogueElement> conversation;
    private int dialogueIndex;
    private bool awaitingChoice = false;

    // typewriter control
    private bool finishedTypewriter;
    private const float DIALOGUE_SPEED = 0.025f;

    // Regex
    private Regex tagPattern = new Regex(@"<[^>]*>"); // Matches rich text tags like <i>text</i>
    private Regex namePattern = new Regex(@"\b(Unnamed|Kairo)\b", RegexOptions.IgnoreCase);
    private string taglessText = "";

    // state
    private string speaker = null;
    private Dictionary<string, Sprite> speakerSprites = new Dictionary<string, Sprite>();
    private string unnamedName = "Kairo"; // placeholder TODO
    private List<string> endingEvents = new List<string>();

    void Start()
    {
        DialogueWindow.SetActive(false);
    }

    public void PlayDialogue(List<DialogueElement> dialogue)
    {
        StopAllCoroutines();
        conversation = dialogue;
        dialogueIndex = -1;
        speaker = null;
        buttonDisplay.SetActive(false);
        speakerSprites = new Dictionary<string, Sprite>();
        endingEvents = new List<string>();
        awaitingChoice = false;
        SetCinematicMode(false);

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
            if (awaitingChoice) return;
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        dialogueIndex++;

        // check if conversation ended
        if (dialogueIndex >= conversation.Count)
        {
            foreach (string dialogueEvent in endingEvents)
            {
                DialogueManager.Instance.TriggerEvent(dialogueEvent);
            }
            DialogueWindow.SetActive(false);
            if(PlayerController.Instance) PlayerController.Instance.SetMovementLock(false);
            return;
        }

        DialogueElement element = conversation[dialogueIndex];

        // Swap out "Unnamed" or "Kairo" for whatever the player named them
        string line = namePattern.Replace(element.Line, unnamedName);
        if (element.Speaker != null)
        {
            speaker = namePattern.Replace(element.Speaker, unnamedName);
        }

        // Remove rich text tags
        taglessText = tagPattern.Replace(line, string.Empty);

        // Play the dialogue
        foreach (string sound in element.Sounds)
        {
            // TODO
        }

        foreach (DialogueEventElement dialogueEvent in element.Events)
        {            
            if (dialogueEvent.triggerAtEnd)
            {
                endingEvents.Add(dialogueEvent.eventName);
            }
            else DialogueManager.Instance.TriggerEvent(dialogueEvent.eventName);
        }

        if (element.FromRadio)
        {
            // TODO
        }

        if (element.Icon != null)
        {
            portrait.sprite = spriteMap.GetSprite(element.Speaker + element.Icon);
        }
        else if (speakerSprites.ContainsKey(speaker))
        {
            portrait.sprite = speakerSprites[speaker];
        }

        if (element.Background != null)
        {
            if (element.Background.ToLower() == "none")
            {
                SetCinematicMode(false);
            }
            else
            {
                backgroundGraphic.sprite = backgroundMap.GetSprite(element.Background);
                SetCinematicMode(true);
            }
        }

        if (element.Choice1 != null && element.Choice2 != null)
        {
            awaitingChoice = true;
            buttonDisplay.SetActive(true);
            SetChoiceButton(choiceButton1, choiceText1, element.Choice1);
            SetChoiceButton(choiceButton2, choiceText2, element.Choice2);
        }

        lineTarget.text = conversation[dialogueIndex].Line;
        speakerTarget.text = speaker; 
        StartCoroutine(TypewriterEffect());
    }

    private void SetChoiceButton(UnityEngine.UI.Button button, TextMeshProUGUI choiceText, DialogueChoice choice)
    {
        choiceText.text = choice.Response;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            // TODO adjust friendship
            if (choice.LinkedDialogue != null)
            {
                DialogueManager.Instance.PlayDialogue(choice.LinkedDialogue);
            }
            buttonDisplay.SetActive(false);
            awaitingChoice = false;
        });
    }

    private void SetCinematicMode(bool isCinematic)
    {
        backgroundGraphic.gameObject.SetActive(isCinematic);
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
