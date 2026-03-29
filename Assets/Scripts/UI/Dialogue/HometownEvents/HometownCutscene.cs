using UnityEngine;
using System.Text.RegularExpressions;
using FMODUnity;

public class HometownCutscene : MonoBehaviour
{
    public Animator Animator;
    public UnityEngine.UI.Image Image;

    public void PlaySound(string sound)
    {
        VoicelineManager.Instance.PlayDialogueAudioEffect(sound);
    }

    public void OnCutsceneEnd()
    {
        EndingManager.Instance.EndCutscene();
    }

    public void MyOnAnimationEnd()
    {
        string nextEvent = AnimationClipToDialogueName(Animator.GetCurrentAnimatorClipInfo(0)[0].clip);
        Debug.Log($"Next event: {nextEvent}");
        if (DialogueManager.Instance.GetDialogueDictionary().ContainsKey(nextEvent))
        {
            DialogueManager.Instance.PlayDialogue(nextEvent);
        }
    }

    // Translates an animation clip's name into a dialogue name for the current ending
    // Example: 'HometownEnterHouse' animation, 'OrangeSunset' ending
    // Output: enter_house_orange_sunset
    private static string AnimationClipToDialogueName(AnimationClip clip)
    {
        string eventName = PascalToSnake(clip.name) + "_" + PascalToSnake(EndingManager.Instance.CurrentEnding.ToString());
        return eventName.Substring(eventName.IndexOf('_') + 1);
    }

    private static string PascalToSnake(string input)
    {
        // Inserts an underscore before each uppercase letter that is not at the beginning of the string, 
        // and is followed by a lowercase letter or is preceded by a lowercase letter and followed by an uppercase letter.
        string snakeCase = Regex.Replace(input, "([A-Z])([A-Z][a-z])|([a-z0-9])([A-Z])", "$1$3_$2$4");
        snakeCase = snakeCase.ToLowerInvariant();

        return snakeCase;
    }

    public void OnEnable()
    {
        Debug.Log("First frame is up");
        RuntimeManager.PlayOneShot(AudioEvents.SFX.wood_door_opens);
        MusicManager.Instance.ChangeMusic(AudioEvents.Music.s4Hometown_death_cutscene, 2.0f);
    }
}
