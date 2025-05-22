using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "VoicelineEventSO", menuName = "VoicelineEventSO", order = 1)]
public class VoicelineEventSO : ScriptableObject
{
    [field: SerializeField] public EventReference voicelineEventReference;

    public static implicit operator EventReference(VoicelineEventSO voicelineEvent)
    {
        if (voicelineEvent == null)
        {
            return default;
        }
        return voicelineEvent.voicelineEventReference;
    }
}