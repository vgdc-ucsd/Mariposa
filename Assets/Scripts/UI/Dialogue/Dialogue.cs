using UnityEngine;
using UnityEngine.UI;
using static System.String;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName="Dialogue", menuName="Dialogue")]
public class Dialogue : ScriptableObject
{
	[SerializeField] public string SpeakerName {get => SpeakerName;}
	[SerializeField] [TextAreaAttribute(1, 3)] public string Line {get => Line;}
}
