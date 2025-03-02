using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName="Dialogue", menuName="Dialogue")]
public class Dialogue : ScriptableObject
{
    // collection of lines of dialogue
    [SerializeField] public List<DialogueElement> Conversation;
}

[System.Serializable]
public class DialogueElement
{
    // text fields
    public string Speaker;
    [TextAreaAttribute(1, 3)] public string Line;

    // character portrait sprite
    public Sprite Sprite;

    // true if dialogue is coming from the radio
    public bool FromRadio;
}