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

    [SerializeField] private TMP_Text speakerTarget;
    [SerializeField] private TMP_Text lineTarget;

    [SerializeField] private Image textboxRect;
    [SerializeField] private Sprite mariRect;
    [SerializeField] private Sprite unnRect;

    [SerializeField] private Image nameplate;
    [SerializeField] private Sprite mariNameplate;
    [SerializeField] private Sprite unnNameplate;

    [SerializeField] private Image portraitBG;

    [SerializeField] private Image radio;
    [SerializeField] private Sprite mariRadio;
    [SerializeField] private Sprite unnRadio;

    [SerializeField] private Image advanceIndicator;
    [SerializeField] private Hoverer advanceHoverer;
    [SerializeField] private Sprite mariAdvance;
    [SerializeField] private Sprite unnAdvance;

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
    private List<DialogueElement> conversation = new List<DialogueElement>();
    private int dialogueIndex = 0;
    private bool awaitingChoice = false;

    // typewriter control
    private bool finishedTypewriter;
    private const float DIALOGUE_SPEED = 0.03f;

    // Regex
    private Regex tagPattern = new Regex(@"<\/?(i|b|color(=[^>]+)?)>"); // Matches rich text tags like <i>text</i>
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

    public void PlayDialogue(List<DialogueElement> dialogue, bool initAdvance)
    {
        StopAllCoroutines();
        conversation = dialogue;
        dialogueIndex = -1;
        speaker = null;
        buttonDisplay.SetActive(false);
        portraitBG.gameObject.SetActive(false);
        radio.gameObject.SetActive(false);
        advanceIndicator.gameObject.SetActive(false);
        speakerSprites = new Dictionary<string, Sprite>();
        endingEvents = new List<string>();
        awaitingChoice = false;
        if (InGameUI.Instance != null) InGameUI.Instance.InteractPrompt(false);
        SetCinematicMode(false);

        DialogueWindow.SetActive(true);
        if (PlayerController.Instance) PlayerController.Instance.SetMovementLock(true);

        // check if Mariposa currently active
        if (!PlayerController.Instance || Player.ActivePlayer.Data.characterID == CharID.Mariposa)
        {
            nameplate.sprite = mariNameplate;
            textboxRect.sprite = mariRect;
            radio.sprite = mariRadio;
            advanceIndicator.sprite = mariAdvance;
        }
        else
        {
            nameplate.sprite = unnNameplate;
            textboxRect.sprite = unnRect;
            radio.sprite = unnRadio;
            advanceIndicator.sprite = unnAdvance;
        }

        if (initAdvance) AdvanceDialogue();
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
            advanceHoverer.Reset();
            advanceIndicator.gameObject.SetActive(true);
        }
        else
        {
            if (awaitingChoice || dialogueIndex == conversation.Count) return;
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        dialogueIndex++;

        // check if conversation ended
        if (dialogueIndex >= conversation.Count)
        {
            VoicelineManager.Instance.StopAllDialogueAudioEffects(FMOD.Studio.STOP_MODE.IMMEDIATE);
            DialogueWindow.SetActive(false);
            if (PlayerController.Instance) PlayerController.Instance.SetMovementLock(false);

            foreach (string dialogueEvent in endingEvents)
            {
                DialogueManager.Instance.TriggerEvent(dialogueEvent);
            }
            
            return;
        }

        VoicelineManager.Instance.StopAllDialogueAudioEffects(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

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
            // removes ".wav" from end of sound string
            string formatted = Regex.Replace(sound, @"\.wav$", "", RegexOptions.IgnoreCase);
            VoicelineManager.Instance.PlayDialogueAudioEffect(formatted);
        }

        foreach (DialogueEventElement dialogueEvent in element.Events)
        {
            if (dialogueEvent.triggerAtEnd)
            {
                endingEvents.Add(dialogueEvent.eventName);
            }
            else DialogueManager.Instance.TriggerEvent(dialogueEvent.eventName);
        }

        if (element.FromRadio != null)
        {
            radio.gameObject.SetActive(element.FromRadio.ToLower() == "on");
        }

        if (element.Icon != null)
        {
            portrait.sprite = spriteMap.GetSprite(element.Speaker + element.Icon);
            portraitBG.gameObject.SetActive(true);
        }
        else if (speakerSprites.ContainsKey(speaker))
        {
            portrait.sprite = speakerSprites[speaker];
            portraitBG.gameObject.SetActive(true);
        }
        else if (speaker == "Mariposa" || element.Speaker == "Unnamed" || speaker == "Beebo")
        {
            portrait.sprite = spriteMap.GetSprite(element.Speaker + "Neutral");
            portraitBG.gameObject.SetActive(true);
        }
        else
        {
            portraitBG.gameObject.SetActive(false);
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
            if (choice.LinkedDialogue != null)
            {
                FriendshipManager.Instance.ChangeScore(choice.Friendship);
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
        advanceIndicator.gameObject.SetActive(false);
        finishedTypewriter = false;
        int length = taglessText.Length;

        int i = 0;
        lineTarget.maxVisibleCharacters = i;
        while (i < length)
        {
            i++;
            lineTarget.maxVisibleCharacters = i;
            bool punctuation = taglessText[i - 1] == ',' || taglessText[i - 1] == '.' || taglessText[i - 1] == '?' || taglessText[i - 1] == '!' || taglessText[i - 1] == ':' || taglessText[i - 1] == ';';
            if (punctuation) yield return new WaitForSeconds(DIALOGUE_SPEED * 10.0f);
            else yield return new WaitForSeconds(DIALOGUE_SPEED);
        }

        if (!awaitingChoice)
        {
            advanceHoverer.Reset();
            advanceIndicator.gameObject.SetActive(true);
        }
        finishedTypewriter = true;
    }
}
