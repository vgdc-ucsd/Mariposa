using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Responsible for coordinating the loading and playing of dialogue
/// </summary>
public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private DialoguePlayer dialoguePlayer;

    private InputSystem_Actions inputs;
    private Dictionary<string, List<DialogueElement>> dialogueDictionary = new Dictionary<string, List<DialogueElement>>();
    private Dictionary<string, DialogueEvent> eventDictionary = new Dictionary<string, DialogueEvent>();

    public override void Awake()
    {
        base.Awake();
        inputs = new InputSystem_Actions();
    }

    /// <summary>
    /// Loads the dialogue data in the given yaml file.
    /// </summary>
    /// <param name="yamlName">The name of the yaml file to be loaded. This should NOT include the .yaml file extension</param>
    public void LoadYaml(string yamlName)
    {
        TextAsset yaml = (TextAsset)Resources.Load($"DialogueData/{yamlName}", typeof(TextAsset));
        if (yaml == null)
        {
            Debug.LogError($"Error loading yaml file {yamlName}! Check that it's spelled correctly and located in Resources/DialogueData!");
            return;
        }
        Dictionary<string, List<DialogueElement>> parsedDialogue = DialogueParser.Parse(yaml);

        if (parsedDialogue.Count == 0)
        {
            Debug.LogWarning($"File {yamlName} contained no dialogue!");
        }

        foreach ((string name, List<DialogueElement> dialogue) in parsedDialogue)
        {
            if (dialogueDictionary.ContainsKey(name))
            {
                Debug.LogWarning($"Dialogue data with the name {name} has already been loaded. This data will be overwritten!");
            }
            dialogueDictionary[name] = dialogue;
        }
    }

    /// <summary>
    /// Begins the dialogue sequence with the matching name.
    /// </summary>
    /// <param name="dialogueName">The name of the dialogue sequence as written in the imported files</param>
    public void PlayDialogue(string dialogueName)
    {
        if (!dialogueDictionary.ContainsKey(dialogueName))
        {
            Debug.LogError($"Could not find dialogue with the name {dialogueName}! Check that there's no typos and the dialogue file has been loaded!");
            return;
        }
        dialoguePlayer.PlayDialogue(dialogueDictionary[dialogueName]);
    }

    private void OnEnable()
    {
        inputs.Enable();
        inputs.Player.Interact.started += ctx => dialoguePlayer.TryAdvanceDialogue();
    }

    private void OnDisable()
    {
        inputs.Player.Interact.started -= ctx => dialoguePlayer.TryAdvanceDialogue();
        inputs.Disable();
    }
}
