using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Responsible for coordinating the loading and playing of dialogue
/// </summary>
public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private DialoguePlayer dialoguePlayer;
    public bool isPlayingDialogue;

    private Dictionary<string, List<DialogueElement>> dialogueDictionary = new Dictionary<string, List<DialogueElement>>();
    private Dictionary<string, DialogueEvent> eventDictionary = new Dictionary<string, DialogueEvent>();

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
    public void PlayDialogue(string dialogueName, bool initAdvance = true)
    {
        if (!dialogueDictionary.ContainsKey(dialogueName))
        {
            Debug.LogError($"Could not find dialogue with the name \"{dialogueName}\"! Check that there's no typos and the dialogue file has been loaded!");
            return;
        }
        dialoguePlayer.PlayDialogue(dialogueDictionary[dialogueName], initAdvance);
        isPlayingDialogue = true;
    }

    public void RegisterEvent(string name, DialogueEvent dialogueEvent)
    {
        if (eventDictionary.ContainsKey(name))
        {
            Debug.LogWarning($"Multiple dialogue events with the name \"{name}\" have been registered! The old event will be overwritten!");
            return;
        }
        eventDictionary.Add(name, dialogueEvent);
    }

    public void TriggerEvent(string eventName)
    {
        if (!eventDictionary.ContainsKey(eventName))
        {
            Debug.LogWarning($"Could not find an event with the name {eventName}! Check that there's no typos and the event has been created in the scene!");
            return;
        }
        eventDictionary[eventName].Trigger();
    }

    public Dictionary<string, List<DialogueElement>> GetDialogueDictionary()
    {
        return dialogueDictionary;
    }

    public void TryAdvanceDialogue() { dialoguePlayer.TryAdvanceDialogue(); }
}
