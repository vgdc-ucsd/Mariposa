using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class VoicelineManager : Singleton<VoicelineManager>
{
    // this is only to be used during dialogue (all other instances should be handled by other respective managers / hardcoded)

    // note: music and ambience commands will be parsed here to allow changes but needs to be written in the script or dialogue event in order to stop

    [SerializeField] private List<EventReference> _playingDialogueVO, _playingSFX;
    private List<EventInstance> playingDialogueVO, playingSFX;

    private enum AudioType
    {
        Music,
        SFX,
        Ambience,
        DialogueVO,
    };

    // TODO: add translations from the script language (key) to the AudioEvent name (value)

    private (EventReference, AudioType) ScriptToAudioEventName(string sound)
    {
        return sound switch
        {
            // Ambience


            // DialogueVO
            "Mariposa_Hmm8" => (AudioEvents.DialogueVO.mariposa_neutral_short_hmm, AudioType.DialogueVO),
            "Mariposa_Alright1" => (AudioEvents.DialogueVO.mariposa_happy_short_alright, AudioType.DialogueVO),
            "Mariposa_IPromise1" => (AudioEvents.DialogueVO.mariposa_happy_medium_i_promise, AudioType.DialogueVO),
            "Mariposa_Giggle3" => (AudioEvents.DialogueVO.mariposa_happy_short_giggle, AudioType.DialogueVO),
            "Mariposa_ThankYou1" => (AudioEvents.DialogueVO.mariposa_happy_medium_thank_you, AudioType.DialogueVO),
            "Mariposa_Hmm5" => (AudioEvents.DialogueVO.mariposa_neutral_short_hmm, AudioType.DialogueVO),
            "Luke_NPC_Well1" => (AudioEvents.DialogueVO.luke_npc_neutral_well, AudioType.DialogueVO),
            "Mariposa_ReadyToGo3" => (AudioEvents.DialogueVO.mariposa_happy_short_ready_to_go, AudioType.DialogueVO),
            "Luke_NPC_AppreciateIt2" => (AudioEvents.DialogueVO.luke_npc_happy_appreciate_it, AudioType.DialogueVO),
            "Mariposa_GladCouldHelp2" => (AudioEvents.DialogueVO.mariposa_happy_medium_glad_could_help, AudioType.DialogueVO),
            "Mariposa_Hmm1" => (AudioEvents.DialogueVO.mariposa_neutral_short_hmm, AudioType.DialogueVO),
            "Mariposa_Sigh2" => (AudioEvents.DialogueVO.mariposa_sad_short_sigh, AudioType.DialogueVO),
            "Luke_NPC_Sigh2" => (AudioEvents.DialogueVO.luke_npc_sad_sigh, AudioType.DialogueVO),
            "Mariposa_Hmm7" => (AudioEvents.DialogueVO.mariposa_neutral_short_hmm, AudioType.DialogueVO),
            "Mariposa_Woah4" => (AudioEvents.DialogueVO.mariposa_surprised_short_woah, AudioType.DialogueVO),
            "Unnamed_Cough1" => (AudioEvents.DialogueVO.unnamed_sfx_cough, AudioType.DialogueVO),
            "Unnamed_TimeToMoveOn4" => (AudioEvents.DialogueVO.unnamed_neutral_time_to_move_on, AudioType.DialogueVO),
            "Unnamed_Ugh3" => (AudioEvents.DialogueVO.unnamed_sad_short_ugh, AudioType.DialogueVO),
            "Luke_NPC_IThink3" => (AudioEvents.DialogueVO.luke_npc_neutral_i_think, AudioType.DialogueVO),
            "Luke_NPC_Hey2" => (AudioEvents.DialogueVO.luke_npc_neutral_hey, AudioType.DialogueVO),
            "Luke_NPC_Hey1" => (AudioEvents.DialogueVO.luke_npc_neutral_hey, AudioType.DialogueVO),
            "Regine_NPC_Okay1" => (AudioEvents.DialogueVO.regine_npc_sad_okay, AudioType.DialogueVO),
            "Regine_NPC_Hm7" => (AudioEvents.DialogueVO.regine_npc_neutral_hm, AudioType.DialogueVO),
            "Regine_NPC_IMean2" => (AudioEvents.DialogueVO.regine_npc_neutral_i_mean, AudioType.DialogueVO),
            "Regine_NPC_Yeah3" => (AudioEvents.DialogueVO.regine_npc_happy_yeah, AudioType.DialogueVO),
            "Luke_NPC_Sigh3" => (AudioEvents.DialogueVO.luke_npc_sad_sigh, AudioType.DialogueVO),
            "Luke_NPC_Dont1" => (AudioEvents.DialogueVO.luke_npc_sad_dont, AudioType.DialogueVO),
            "Luke_NPC_Sigh1" => (AudioEvents.DialogueVO.luke_npc_sad_sigh, AudioType.DialogueVO),

            "Unnamed_Cough2" => (AudioEvents.DialogueVO.unnamed_sfx_cough, AudioType.DialogueVO),
            "Regine_NPC_Honestly1" => (AudioEvents.DialogueVO.regine_npc_neutral_honestly, AudioType.DialogueVO),
            "Ruby_NPC_Woah5" => (AudioEvents.DialogueVO.ruby_npc_surprised_woah, AudioType.DialogueVO),
            "Regine_NPC_IMean3" => (AudioEvents.DialogueVO.regine_npc_neutral_i_mean, AudioType.DialogueVO),
            "Regine_NPC_IThink1" => (AudioEvents.DialogueVO.regine_npc_neutral_i_think, AudioType.DialogueVO),
            "Luke_NPC_Well3" => (AudioEvents.DialogueVO.luke_npc_neutral_well, AudioType.DialogueVO),
            "Justin_NPC_Okay1" => (AudioEvents.DialogueVO.justin_npc_sad_ok, AudioType.DialogueVO),
            "Luke_NPC_Yeah2" => (AudioEvents.DialogueVO.luke_npc_happy_yeah, AudioType.DialogueVO),
            "Regine_NPC_Hm3" => (AudioEvents.DialogueVO.regine_npc_neutral_hm, AudioType.DialogueVO),
            "Regine_NPC_Sigh2" => (AudioEvents.DialogueVO.regine_npc_sad_sigh, AudioType.DialogueVO),
            "Regine_NPC_Sigh3" => (AudioEvents.DialogueVO.regine_npc_sad_sigh, AudioType.DialogueVO),
            "Justin_NPC_Laugh7" => (AudioEvents.DialogueVO.justin_npc_happy_laugh, AudioType.DialogueVO),
            "Justin_NPC_Laugh2" => (AudioEvents.DialogueVO.justin_npc_happy_laugh, AudioType.DialogueVO),
            "Luke_Robot_NPC_Aww" => (AudioEvents.DialogueVO.luke_npc_robot_aww, AudioType.DialogueVO),
            "Regine_NPC_Mari2" => (AudioEvents.DialogueVO.regine_npc_greetings_mari, AudioType.DialogueVO),
            "Regine_NPC_Hm1" => (AudioEvents.DialogueVO.regine_npc_neutral_hm, AudioType.DialogueVO),

            "Ruby_NPC_HiThere2" => (AudioEvents.DialogueVO.ruby_npc_greetings_hi_there, AudioType.DialogueVO),
            "Ruby_NPC_YouOkay1" => (AudioEvents.DialogueVO.ruby_npc_surprised_you_okay, AudioType.DialogueVO),
            "Regine_NPC_Hm4" => (AudioEvents.DialogueVO.regine_npc_neutral_hm, AudioType.DialogueVO),
            "Luke_NPC_LetsThinkAboutThis1" => (AudioEvents.DialogueVO.luke_npc_neutral_lets_think_about_this, AudioType.DialogueVO),

            "Justin_NPC_Laugh1" => (AudioEvents.DialogueVO.justin_npc_happy_laugh, AudioType.DialogueVO),
            "Ruby_NPC_Mariposa4" => (AudioEvents.DialogueVO.ruby_npc_greetings_mariposa, AudioType.DialogueVO),
            "Mariposa_Woah3" => (AudioEvents.DialogueVO.mariposa_surprised_short_woah, AudioType.DialogueVO),
            "Ruby_NPC_Hm2" => (AudioEvents.DialogueVO.ruby_npc_neutral_hm, AudioType.DialogueVO),
            "Ruby_NPC_Giggle-Laugh1" => (AudioEvents.DialogueVO.ruby_npc_happy_giggle_laugh, AudioType.DialogueVO),

            // Music


            // SFX
            //"modular-static-evil-beebo" => "",
            //"Earthquake Full Shortened" => "",

            //"fix_radio" => "",
            //"radio-clip3-novoices" => "event:/sfx/item/radio/static_dialogue",              // they wanted a shorter radio clip

            // Default
            _ => default
        };
    }

    private void Start()
    {
        playingDialogueVO = new List<EventInstance>();
        playingSFX = new List<EventInstance>();
        _playingDialogueVO = new List<EventReference>();
        _playingSFX = new List<EventReference>();
    }

    public void PlayDialogueAudioEffect(string sound)
    {
        var (eventRef, audioType) = ScriptToAudioEventName(sound);

        if (eventRef.Equals(default) || eventRef.IsNull)
        {
            Debug.LogWarning($"The dialogue audio event '{sound}' is invalid! Ignoring...");
            return;
        }

        switch (audioType)
        {
            case AudioType.Music:
                ChangeMusic(eventRef);
                break;
            case AudioType.SFX:
                AddSFX(eventRef);
                break;
            case AudioType.Ambience:
                ChangeAmbience(eventRef);
                break;
            case AudioType.DialogueVO:
                AddDialogueVO(eventRef);
                break;
            default: 
                Debug.LogError($"The dialogue audio event '{sound}' did not match any enum's! Faulty dictionary!");
                break;
        }
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
