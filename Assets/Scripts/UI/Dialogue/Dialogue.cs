using UnityEngine;
using System.Collections.Generic;

public enum DialogueImpactSize
{
	ImpactLarge,
	ImpactMedium,
	ImpactSmall,
}

public enum DialogueImpactType 
{
	ImpactPositive,
	ImpactNegative,
}

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

[System.Serializable]
public class DialogueOption
{
	public string Speaker;

	[TextAreaAttribute(1, 3)] public string OptionName;

	public DialogueImpactSize RelationImpactSignificance;
	public DialogueImpactType RelationImpactDirection;

    [SerializeField] public List<DialogueElement> Conversation;
}
