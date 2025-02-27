using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName="Dialogue", menuName="Dialogue")]
public class Dialogue : ScriptableObject
{
    [SerializeField] public List<DialogueElement> Conversation;
}

[System.Serializable]
public class DialogueElement
{
    public string Speaker;
    [TextAreaAttribute(1, 3)] public string Line;

    public Sprite Sprite;
}