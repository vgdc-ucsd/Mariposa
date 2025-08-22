using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class VoicelineManager : Singleton<VoicelineManager>
{
    // this is only to be used during dialogue (all other instances should be handled by other respective managers / hardcoded)

    // note: music and ambience commands will be parsed here to allow changes but needs to be written in the script or dialogue event in order to stop

    [SerializeField] private List<EventReference> _playingDialogueVO, _playingSFX;
    private List<EventInstance> playingDialogueVO, playingSFX;

    // TODO: add translations from the script language (key) to the AudioEvent name (value)

    private EventReference ScriptToAudioEventName(string sound)
    {
        if (sound.StartsWith("Mariposa"))
        {
            return (sound["Mariposa".Length..]) switch
            {
                "Mariposa_Alright2" => AudioEvents.DialogueVO.mariposa_happy_short_alright,
                "Mariposa_Alright3" => AudioEvents.DialogueVO.mariposa_happy_short_alright,
                "Mariposa_Aww2" => AudioEvents.DialogueVO.mariposa_sad_short_aww,
                "Mariposa_Giggle1" => AudioEvents.DialogueVO.mariposa_happy_short_giggle,
                "Mariposa_Giggle2" => AudioEvents.DialogueVO.mariposa_happy_short_giggle,
                "Mariposa_Giggle4" => AudioEvents.DialogueVO.mariposa_happy_short_giggle,
                "Mariposa_Giggle6" => AudioEvents.DialogueVO.
                "Mariposa_GladCouldHelp2" => AudioEvents.DialogueVO.
                "Mariposa_Hey2" => AudioEvents.DialogueVO.
                "Mariposa_Hmm1" => AudioEvents.DialogueVO.
                "Mariposa_Hmm2" => AudioEvents.DialogueVO.
                "Mariposa_Hmm3" => AudioEvents.DialogueVO.
                "Mariposa_Hmm4" => AudioEvents.DialogueVO.
                "Mariposa_Hmm5" => AudioEvents.DialogueVO.
                "Mariposa_Hmm6" => AudioEvents.DialogueVO.
                "Mariposa_Hmm7" => AudioEvents.DialogueVO.
                "Mariposa_HowBoutThis1" => AudioEvents.DialogueVO.
                "Mariposa_HowBoutThis8" => AudioEvents.DialogueVO.
                "Mariposa_IMean1" => AudioEvents.DialogueVO.
                "Mariposa_IPromise1" => AudioEvents.DialogueVO.
                "Mariposa_InOrder3" => AudioEvents.DialogueVO.
                "Mariposa_ItWorks1" => AudioEvents.DialogueVO.
                "Mariposa_No1" => AudioEvents.DialogueVO.
                "Mariposa_NothingICanDo2" => AudioEvents.DialogueVO.
                "Mariposa_NothingICanDo4" => AudioEvents.DialogueVO.
                "Mariposa_PieceOfCake3" => AudioEvents.DialogueVO.
                "Mariposa_ReadyToGo3" => AudioEvents.DialogueVO.
                "Mariposa_SeeYaRound1" => AudioEvents.DialogueVO.
                "Mariposa_Seriously3" => AudioEvents.DialogueVO.
                "Mariposa_Seriously5" => AudioEvents.DialogueVO.
                "Mariposa_Sigh2" => AudioEvents.DialogueVO.
                "Mariposa_Sigh4" => AudioEvents.DialogueVO.
                "Mariposa_Sorry3" => AudioEvents.DialogueVO.
                "Mariposa_ThankYou1" => AudioEvents.DialogueVO.
                "Mariposa_UpAndRunning1" => AudioEvents.DialogueVO.
                "Mariposa_Urgh3" => AudioEvents.DialogueVO.
                "Mariposa_Urgh4" => AudioEvents.DialogueVO.
                "Mariposa_Well2" => AudioEvents.DialogueVO.
                "Mariposa_Well3" => AudioEvents.DialogueVO.
                "Mariposa_Well4" => AudioEvents.DialogueVO.
                "Mariposa_WhyNoWork1" => AudioEvents.DialogueVO.
                "Mariposa_Woah1" => AudioEvents.DialogueVO.
                "Mariposa_Woah3" => AudioEvents.DialogueVO.
                "Mariposa_Woah4" => AudioEvents.DialogueVO.
                "Mariposa_Yeah1" => AudioEvents.DialogueVO.
                "Mariposa_Yeah3" => AudioEvents.DialogueVO.
                "Mariposa_YouOk1" => AudioEvents.DialogueVO.

            };
        }
        else if (sound.StartsWith("Unnamed"))
        {

        }
        else if (sound.StartsWith("Luke_NPC"))
        {

        }
        else if (sound.StartsWith("Regine_NPC"))
        {

        }
        else if (sound.StartsWith("Ruby_NPC"))
        {

        }
        else if (sound.StartsWith("Justin_NPC"))
        {

        }
        else
        {
            return default;
        }
    }

    private void Start()
    {
        playingDialogueVO = new List<EventInstance>();
        playingSFX = new List<EventInstance>();
        _playingDialogueVO = new List<EventReference>();
        _playingSFX = new List<EventReference>();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Invalid audio events:");
        foreach (string audio in invalidAudioEvents) Debug.Log($"{audio}");
    }

    List<string> invalidAudioEvents = new List<string>();
    public void PlayDialogueAudioEffect(string sound)
    {
        var eventRef  = ScriptToAudioEventName(sound);

        if (eventRef.Equals(default) || eventRef.IsNull)
        {
            invalidAudioEvents.Add(sound);
            Debug.LogWarning($"The dialogue audio event '{sound}' is invalid! Ignoring...");
            return;
        }

        AddDialogueVO(eventRef);
    }

    public void StopAllDialogueAudioEffects(FMOD.Studio.STOP_MODE stopMode)
    {
        foreach (EventInstance vo in playingDialogueVO)
        {
            AudioManager.StopEventInstance(vo, stopMode);
        }
        playingDialogueVO.Clear();
        _playingDialogueVO.Clear();

        foreach (EventInstance sfx in playingSFX)
        {
            AudioManager.StopEventInstance(sfx, stopMode);
        }
        playingSFX.Clear();
        _playingSFX.Clear();

        // note: this does not auto-stop the music or ambience (please use the respective manager for it: AmbienceManager / MusicManager)
    }

    private void ChangeMusic(EventReference music)
    {
        MusicManager.Instance.ChangeMusic(music);
    }

    private void ChangeAmbience(EventReference ambience)
    {
        AmbienceManager.Instance.ChangeAmbience(ambience);
    }

    private void AddSFX(EventReference sfx)
    {
        EventInstance sfxInstance = AudioManager.CreateEventInstance(sfx);
        if (!sfxInstance.isValid())
        {
            Debug.LogError($"The dialogue sfx event '{sfx.Path}' has an invalid path! Faulty AudioEvent! Skipping...");
            return;
        }

        sfxInstance.start();
        _playingSFX.Add(sfx);
        playingSFX.Add(sfxInstance);
    }

    private void AddDialogueVO(EventReference vo)
    {
        EventInstance voInstance = AudioManager.CreateEventInstance(vo);
        if (!voInstance.isValid())
        {
            Debug.LogError($"The dialogue voiceover event '{vo.Path}' has an invalid path! Faulty AudioEvent! Skipping...");
            return;
        }
        voInstance.start();
        _playingDialogueVO.Add(vo);
        playingDialogueVO.Add(voInstance);
    }
}
