using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueElement
{
    public string Speaker; // The speaker, assume previous speaker if empty
    public string Line; // The line
    public string Icon; // The character portrait, assume previous icon if empty
    public string Background; // The background graphic
    public bool FromRadio; // Indicates whether to show the radio icon on the dialogue UI
    public List<string> Sounds; // A list of sound effects to play 
    public List<DialogueEventElement> Events; // A list of arbitrary named events to trigger (see DialogueEvent.cs)
    public DialogueChoice Choice1; // Dialogue response options
    public DialogueChoice Choice2; // No responses options if null

    public DialogueElement()
    {
        Speaker = null;
        Line = null;
        Icon = null;
        Background = null;
        FromRadio = false;
        Sounds = new List<string>();
        Events = new List<DialogueEventElement>();
        Choice1 = null;
        Choice2 = null;
    }
}

public class DialogueChoice
{
    public string Response; // The text to be displayed for the choice
    public string LinkedDialogue; // The name of the next dialogue to play if this response is chosen, dialogue ends if empty
    public int Friendship; // How this response influcences Mariposa & Unnamed's friendship, positive values increase and negative decrease 

    public DialogueChoice()
    {
        Response = null;
        LinkedDialogue = null;
        Friendship = 0;
    }
}

public class DialogueEventElement
{
    public string eventName;
    public bool triggerAtEnd;

    public DialogueEventElement()
    {
        eventName = null;
        triggerAtEnd = false;
    }
}