using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DialoguePlayer : MonoBehaviour
{
    // object references
    [SerializeField] private GameObject defaultDialogueWindow;
    [SerializeField] private GameObject cinematicDialogueWindow;

    [SerializeField] private TMP_Text speakerTarget;
    [SerializeField] private TMP_Text defaultLineTarget;
    [SerializeField] private TMP_Text cinematicLineTarget;

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
    [SerializeField] private Transform portraitStart;
    [SerializeField] private Transform portraitMain;
    [SerializeField] private Transform portraitOut;

    [SerializeField] private Image backgroundGraphic;
    [SerializeField] private SpriteMap backgroundMap;

    [SerializeField] private GameObject buttonDisplay;
    [SerializeField] private UnityEngine.UI.Button choiceButton1;
    [SerializeField] private UnityEngine.UI.Button choiceButton2;
    [SerializeField] private TextMeshProUGUI choiceText1;
    [SerializeField] private TextMeshProUGUI choiceText2;

    private TMP_Text activeLineTarget;
    private GameObject activeDialogueWindow;

    // Portrait animation
    private Coroutine portraitTransition;
    private const float TRANSITION_TIME = 0.4f;

    // dialogue control
    private List<DialogueElement> conversation = new List<DialogueElement>();
    private int dialogueIndex = 0;
    private bool awaitingChoice = false;
    private bool isFading = false;
    private bool acceptingInput = false;
    private bool isCinematic = false;

    // typewriter control
    private bool finishedTypewriter;
    private Coroutine typewriterEffect;
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
        activeDialogueWindow = defaultDialogueWindow;
        activeLineTarget = defaultLineTarget;
        activeDialogueWindow.SetActive(false);
    }

    public void PlayDialogue(List<DialogueElement> dialogue, bool initAdvance)
    {
        if (typewriterEffect != null) StopCoroutine(typewriterEffect);
        conversation = dialogue;
        dialogueIndex = -1;
        speaker = null;
        buttonDisplay.SetActive(false);
        portraitBG.gameObject.SetActive(false);
        portraitBG.sprite = null;
        radio.gameObject.SetActive(false);
        advanceIndicator.gameObject.SetActive(false);
        speakerSprites = new Dictionary<string, Sprite>();
        endingEvents = new List<string>();
        awaitingChoice = false;
        isFading = false;
        acceptingInput = false;
        if (InGameUI.Instance != null) InGameUI.Instance.InteractPrompt(false);

        // TODO: hard-coded workaround for final hometown cutscene
        if (EndingManager.Instance && !EndingManager.Instance.IsCutsceneActive) SetCinematicMode(false);
        else activeLineTarget.text = "";

        activeDialogueWindow.SetActive(true);
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

        StartCoroutine(ReenableInput());
        if (initAdvance) AdvanceDialogue();
    }

    public void EndCutscene()
    {
        SetCinematicMode(false);
        PlayerController.Instance.SetMovementLock(false);
    }

    public void TryAdvanceDialogue()
    {
        // If currently fading, in cinematic mode, or at the very start of a dialogue, don't advance
        if (isFading || !acceptingInput) return;

        // if typewriter effect not finished yet
        if (!finishedTypewriter)
        {
            // finish typewriter effect
            if (typewriterEffect != null) StopCoroutine(typewriterEffect);
            finishedTypewriter = true;
            activeLineTarget.maxVisibleCharacters = taglessText.Length;
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
            activeDialogueWindow.SetActive(false);

            // Removed for hometown cutscenes
            // backgroundGraphic.gameObject.SetActive(false);

            if (PlayerController.Instance && !isCinematic) PlayerController.Instance.SetMovementLock(false);

            foreach (string dialogueEvent in endingEvents)
            {
                DialogueManager.Instance.TriggerEvent(dialogueEvent);
            }

            DialogueManager.Instance.isPlayingDialogue = false;
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
            PlayPortraitTransition(PortraitEnter());
        }
        else if (speakerSprites.ContainsKey(speaker))
        {
            portrait.sprite = speakerSprites[speaker];
            portrait.color = Color.white;
            portraitBG.gameObject.SetActive(true);
        }
        else if (speaker == "Mariposa" || element.Speaker == "Unnamed" || speaker == "Beebo")
        {
            Sprite neutral = spriteMap.GetSprite(element.Speaker + "Neutral");
            if (portrait.sprite != neutral)
            {
                portrait.sprite = neutral;
                PlayPortraitTransition(PortraitEnter());
            }
            else
            {
                portrait.sprite = neutral;
                portrait.color = Color.white;
                portraitBG.gameObject.SetActive(true);
            }
        }
        else
        {
            PlayPortraitTransition(PortraitExit());
        }

        if (element.Background != null && !EndingManager.Instance.IsCutsceneActive)
        {
            if (element.Background.ToLower() == "none")
            {
                SetCinematicMode(false);
            }
            else
            {
                // Wait for screen to fade before continuing execution
                isFading = true;
                FadeController.Instance.FadeOutAndDo(() =>
                {
                    backgroundGraphic.sprite = backgroundMap.GetSprite(element.Background);
                    SetCinematicMode(true);
                    FadeController.Instance.FadeIn();
                    isFading = false;

                    activeLineTarget.text = conversation[dialogueIndex].Line;
                    speakerTarget.text = speaker;
                    typewriterEffect = StartCoroutine(TypewriterEffect());
                });
            }
        }
        else
        {
            // If no background change, execute text update immediately
            activeLineTarget.text = conversation[dialogueIndex].Line;
            speakerTarget.text = speaker;
            typewriterEffect = StartCoroutine(TypewriterEffect());
        }

        if (element.Choice1 != null && element.Choice2 != null)
        {
            awaitingChoice = true;
            buttonDisplay.SetActive(true);
            SetChoiceButton(choiceButton1, choiceText1, element.Choice1);
            SetChoiceButton(choiceButton2, choiceText2, element.Choice2);
        }
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
        this.isCinematic = isCinematic;
        backgroundGraphic.gameObject.SetActive(isCinematic);
        activeLineTarget = isCinematic ? cinematicLineTarget : defaultLineTarget;
        activeDialogueWindow = isCinematic ? cinematicDialogueWindow : defaultDialogueWindow;
        defaultDialogueWindow.SetActive(!isCinematic);
        cinematicDialogueWindow.SetActive(isCinematic);
    }

    private IEnumerator TypewriterEffect()
    {
        advanceIndicator.gameObject.SetActive(false);
        finishedTypewriter = false;
        int length = taglessText.Length;

        int i = 0;
        activeLineTarget.maxVisibleCharacters = i;
        while (i < length)
        {
            i++;
            activeLineTarget.maxVisibleCharacters = i;
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

    private void OnDestroy()
    {
        if (portraitTransition != null) StopCoroutine(portraitTransition);
    }

    private IEnumerator ReenableInput()
    {
        yield return new WaitForSeconds(0.1f);
        acceptingInput = true;
    }

    private void PlayPortraitTransition(IEnumerator transition)
    {
        if (portraitTransition != null) StopCoroutine(portraitTransition);
        portraitTransition = StartCoroutine(transition);
    }

    private IEnumerator PortraitEnter()
    {
        yield return BasicAnimations.Interpolate
        (
            () => portraitBG.gameObject.SetActive(true),
            (t) =>
            {
                float curve = BasicAnimations.EaseOut(t);
                portrait.color = new Color(1.0f, 1.0f, 1.0f, curve);
                portrait.transform.position = Vector3.Lerp
                (
                    portraitStart.transform.position,
                    portraitMain.transform.position,
                    curve
                );
            },
            () => portrait.transform.position = portraitMain.transform.position,
            TRANSITION_TIME
        );
    }
    
    private IEnumerator PortraitExit()
    {
        yield return BasicAnimations.Interpolate
        (
            null,
            (t) => {
                float curve = BasicAnimations.EaseOut(t);
                portrait.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - curve);
                portrait.transform.position = Vector3.Lerp
                (
                    portraitMain.transform.position,
                    portraitOut.transform.position,
                    curve
                );
            },
            () => portraitBG.gameObject.SetActive(false),
            TRANSITION_TIME
        );
    }
}
