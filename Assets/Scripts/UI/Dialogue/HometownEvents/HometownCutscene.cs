using UnityEngine;
using UnityEngine.UI;

public class HometownCutscene : MonoBehaviour
{
    public Animator Animator;
    public Image Image;

    public void PlaySound(string sound)
    {
        VoicelineManager.Instance.PlayDialogueAudioEffect(sound);
    }

    public void OnCutsceneEnd()
    {
        // TODO: go to credits, or main menu?
        Debug.Log("Game finished");
    }
}
