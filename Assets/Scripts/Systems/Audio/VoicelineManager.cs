using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using static AudioEvents;

public class VoicelineManager : Singleton<VoicelineManager>
{
    // this is only to be used during dialogue (all other instances should be handled by other respective managers / hardcoded)

    // note: music and ambience commands will be parsed here to allow changes but needs to be written in the script or dialogue event in order to stop

    [SerializeField] private List<string> _playingDialogueVO, _playingSFX;
    private List<EventInstance> playingDialogueVO, playingSFX;

    // TODO: add translations from the script language (key) to the AudioEvent name (value)

    private string scriptToAudioEventName(string sound)
    {
        return sound switch
        {
            // Ambience


            // DialogueVO
            "Mariposa_Hmm8" => "mariposa_neutral_short_hmm",
            "Mariposa_Alright1" => "mariposa_happy_short_alright",
            "Mariposa_IPromise1" => "mariposa_happy_medium_i_promise",
            "Mariposa_Giggle3" => "mariposa_happy_short_giggle",
            "Mariposa_ThankYou1" => "mariposa_happy_medium_thank_you",
            "Mariposa_Hmm5" => "mariposa_neutral_short_hmm",
            "Luke_NPC_Well1" => "luke_npc_neutral_well",
            "Mariposa_ReadyToGo3" => "mariposa_happy_short_ready_to_go",
            "Luke_NPC_AppreciateIt2" => "luke_npc_happy_appreciate_it",
            "Mariposa_GladCouldHelp2" => "mariposa_happy_medium_glad_could_help",
            "Mariposa_Hmm1" => "mariposa_neutral_short_hmm",
            "Mariposa_Sigh2" => "mariposa_sad_short_sigh",
            "Luke_NPC_Sigh2" => "luke_npc_sad_sigh",
            "Mariposa_Hmm7" => "mariposa_neutral_short_hmm",
            "Mariposa_Woah4" => "mariposa_surprised_short_woah",
            "Unnamed_Cough1" => "unnamed_sfx_cough",
            "Unnamed_TimeToMoveOn4" => "unnamed_neutral_time_to_move_on",
            "Unnamed_Ugh3" => "unnamed_sad_short_ugh",
            "Luke_NPC_IThink3" => "luke_npc_neutral_i_think",
            "Luke_NPC_Hey2" => "luke_npc_neutral_hey",
            "Luke_NPC_Hey1" => "luke_npc_neutral_hey",
            "Regine_NPC_Okay1" => "regine_npc_sad_okay",
            "Regine_NPC_Hm7" => "regine_npc_neutral_hm",
            "Regine_NPC_IMean2" => "regine_npc_neutral_i_mean",
            "Regine_NPC_Yeah3" => "regine_npc_happy_yeah",
            "Luke_NPC_Sigh3" => "luke_npc_sad_sigh",
            "Luke_NPC_Dont1" => "luke_npc_sad_dont",
            "Luke_NPC_Sigh1" => "luke_npc_sad_sigh",

            "Unnamed_Cough2" => "unnamed_sfx_cough",
            "Regine_NPC_Honestly1" => "regine_npc_neutral_honestly",
            "Ruby_NPC_Woah5" => "ruby_npc_surprised_woah",
            "Regine_NPC_IMean3" => "regine_npc_neutral_i_mean",
            "Regine_NPC_IThink1" => "regine_npc_neutral_i_think",
            "Luke_NPC_Well3" => "luke_npc_neutral_well",
            "Justin_NPC_Okay1" => "justin_npc_sad_ok",
            "Luke_NPC_Yeah2" => "luke_npc_happy_yeah",
            "Regine_NPC_Hm3" => "regine_npc_neutral_hm",
            "Regine_NPC_Sigh2" => "regine_npc_sad_sigh",
            "Regine_NPC_Sigh3" => "regine_npc_sad_sigh",
            "Justin_NPC_Laugh7" => "justin_npc_happy_laugh",
            "Justin_NPC_Laugh2" => "justin_npc_happy_laugh",
            "Luke_Robot_NPC_Aww" => "luke_npc_robot_aww",
            "Regine_NPC_Mari2" => "regine_npc_greetings_mari",
            "Regine_NPC_Hm1" => "regine_npc_neutral_hm",

            "Ruby_NPC_HiThere2" => "ruby_npc_greetings_hi_there",
            "Ruby_NPC_YouOkay1" => "ruby_npc_surprised_you_okay",
            "Regine_NPC_Hm4" => "regine_npc_neutral_hm",
            "Luke_NPC_LetsThinkAboutThis1" => "luke_npc_neutral_lets_think_about_this",

            "Justin_NPC_Laugh1" => "justin_npc_happy_laugh",
            "Ruby_NPC_Mariposa4" => "ruby_npc_greetings_mariposa",
            "Mariposa_Woah3" => "mariposa_surprised_short_woah",
            "Ruby_NPC_Hm2" => "ruby_npc_neutral_hm",
            "Ruby_NPC_Giggle-Laugh1" => "ruby_npc_happy_giggle_laugh",

            // Music


            // SFX
            "modular-static-evil-beebo" => "",
            "Earthquake Full Shortened" => "",

            "fix_radio" => "",
            "radio-clip3-novoices" => "event:/sfx/item/radio/static_dialogue",              // they wanted a shorter radio clip

            // Default
            _ => ""
        };
    }

    private void Start()
    {
        playingDialogueVO = new List<EventInstance>();
        playingSFX = new List<EventInstance>();
        _playingDialogueVO = new List<string>();
        _playingSFX = new List<string>();
    }

    public void PlayDialogueAudioEffect(string sound)
    {
        string audioEventName = scriptToAudioEventName(sound);

        if (audioEventName == null || audioEventName == "")
        {
            Debug.LogWarning($"The dialogue audio event '{sound}' is invalid! Ignoring...");
        }
        else if (Enum.TryParse(audioEventName, out Ambience ambience))
        {
            changeAmbience(ambience);
            //Debug.Log($"{ambience.GetPath()}");
        }
        else if (Enum.TryParse(audioEventName, out DialogueVO vo))
        {
            addDialogueVO(vo);
            //Debug.Log($"{vo.GetPath()}");
        }
        else if (Enum.TryParse(audioEventName, out Music music))
        {
            changeMusic(music);
            //Debug.Log($"{music.GetPath()}");
        }
        else if (Enum.TryParse(audioEventName, out SFX sfx))
        {
            addSFX(sfx);
            //Debug.Log($"{sfx.GetPath()}");
        }
        else
        {
            Debug.LogError($"The dialogue audio event '{sound}' did not match any enum's! Faulty dictionary!");
        }
    }

    public void StopAllDialogueAudioEffects(FMOD.Studio.STOP_MODE stopMode)
    {
        foreach (EventInstance vo in playingDialogueVO)
        {
            vo.stop(stopMode);
            vo.release();
        }
        playingDialogueVO.Clear();
        _playingDialogueVO.Clear();

        foreach (EventInstance sfx in playingSFX)
        {
            sfx.stop(stopMode);
            sfx.release();
        }
        playingSFX.Clear();
        _playingSFX.Clear();

        // note: this does not auto-stop the music or ambience (please use the respective manager for it: AmbienceManager / MusicManager)
    }

    private void changeMusic(Music music)
    {
        MusicManager.Instance.ChangeMusic(music);
    }

    private void changeAmbience(Ambience ambience)
    {
        AmbienceManager.Instance.ChangeAmbience(ambience);
    }

    private void addSFX(SFX sfx)
    {
        EventInstance sfxInstance = RuntimeManager.CreateInstance(sfx.GetPath());
        if (!sfxInstance.isValid())
        {
            Debug.LogError($"The dialogue sfx event '{sfx.GetPath()}' has an invalid path! Faulty AudioEvent! Skipping...");
            return;
        }
        sfxInstance.start();
        _playingSFX.Add(sfx.GetPath());
        playingSFX.Add(sfxInstance);
    }

    private void addDialogueVO(DialogueVO vo)
    {
        EventInstance voInstance = RuntimeManager.CreateInstance(vo.GetPath());
        if (!voInstance.isValid())
        {
            Debug.LogError($"The dialogue voiceover event '{vo.GetPath()}' has an invalid path! Faulty AudioEvent! Skipping...");
            return;
        }
        voInstance.start();
        _playingDialogueVO.Add(vo.GetPath());
        playingDialogueVO.Add(voInstance);
    }
}