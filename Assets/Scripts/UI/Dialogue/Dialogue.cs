using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueElement
{
    public string Speaker; // The speaker, assume previous speaker if empty
    public string Line; // The line
    public string Icon; // The character portrait, assume previous icon if empty
    public bool FromRadio; // Indicates whether to show the radio icon on the dialogue UI
    public List<string> Sounds; // A list of sound effects to play 
    public List<string> Events; // A list of arbitrary named events to trigger (see DialogueEvent.cs)
    public List<DialogueChoice> Choices; // A list of responses that the player can choose, no responses if empty
}

public class DialogueChoice
{
    public string Choice; // The text to be displayed for the choice
    public string LinkedDialogue; // The name of the next dialogue to play if this response is chosen, dialogue ends if empty
    public int Friendship; // How this response influcences Mariposa & Unnamed's friendship, positive values increase and negative decrease 
    public List<string> Events; // A list of arbitrary named events to trigger (see DialogueEvent.cs)
}