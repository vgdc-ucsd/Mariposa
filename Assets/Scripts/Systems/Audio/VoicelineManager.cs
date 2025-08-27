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

    [SerializeField] private List<EventReference> playingDialogueVOEvent;
    private List<EventInstance> playingDialogueVOInstance;

    private static readonly Dictionary<string, EventReference> dialogueAudioEvents = new()
    {
        {"Mariposa_Alright2", AudioEvents.DialogueVO.mariposa_happy_short_alright},
        {"Mariposa_Alright3", AudioEvents.DialogueVO.mariposa_happy_short_alright},
        {"Mariposa_Aww2", AudioEvents.DialogueVO.mariposa_sad_short_aww},
        {"Mariposa_Giggle1", AudioEvents.DialogueVO.mariposa_happy_short_giggle},
        {"Mariposa_Giggle2", AudioEvents.DialogueVO.mariposa_happy_short_giggle},
        {"Mariposa_Giggle4", AudioEvents.DialogueVO.mariposa_happy_short_giggle},
        {"Mariposa_Giggle6", AudioEvents.DialogueVO.mariposa_happy_short_giggle},
        {"Mariposa_GladCouldHelp2", AudioEvents.DialogueVO.mariposa_happy_medium_glad_could_help},
        {"Mariposa_Hey2", AudioEvents.DialogueVO.mariposa_surprised_short_hey},
        {"Mariposa_Hmm1", AudioEvents.DialogueVO.mariposa_neutral_short_hmm},
        {"Mariposa_Hmm2", AudioEvents.DialogueVO.mariposa_neutral_short_hmm},
        {"Mariposa_Hmm3", AudioEvents.DialogueVO.mariposa_neutral_short_hmm},
        {"Mariposa_Hmm4", AudioEvents.DialogueVO.mariposa_neutral_short_hmm},
        {"Mariposa_Hmm5", AudioEvents.DialogueVO.mariposa_neutral_short_hmm},
        {"Mariposa_Hmm6", AudioEvents.DialogueVO.mariposa_neutral_short_hmm},
        {"Mariposa_Hmm7", AudioEvents.DialogueVO.mariposa_neutral_short_hmm},
        {"Mariposa_HowBoutThis1", AudioEvents.DialogueVO.mariposa_neutral_medium_how_bout_this},
        {"Mariposa_HowBoutThis8", AudioEvents.DialogueVO.mariposa_neutral_medium_how_bout_this},
        {"Mariposa_IMean1", AudioEvents.DialogueVO.mariposa_neutral_short_i_mean},
        {"Mariposa_IPromise1", AudioEvents.DialogueVO.mariposa_happy_medium_i_promise},
        {"Mariposa_InOrder3", AudioEvents.DialogueVO.mariposa_happy_medium_in_order},
        {"Mariposa_ItWorks1", AudioEvents.DialogueVO.mariposa_happy_short_it_works},
        {"Mariposa_No1", AudioEvents.DialogueVO.mariposa_sad_short_no},
        {"Mariposa_NothingICanDo2", AudioEvents.DialogueVO.mariposa_sad_medium_nothing_i_can_do},
        {"Mariposa_NothingICanDo4", AudioEvents.DialogueVO.mariposa_sad_medium_nothing_i_can_do},
        {"Mariposa_PieceOfCake3", AudioEvents.DialogueVO.mariposa_happy_medium_piece_of_cake},
        {"Mariposa_ReadyToGo3", AudioEvents.DialogueVO.mariposa_happy_short_ready_to_go},
        {"Mariposa_SeeYaRound1", AudioEvents.DialogueVO.mariposa_happy_medium_see_ya_round},
        {"Mariposa_Seriously3", AudioEvents.DialogueVO.mariposa_surprised_short_seriously},
        {"Mariposa_Seriously5", AudioEvents.DialogueVO.mariposa_surprised_short_seriously},
        {"Mariposa_Sigh2", AudioEvents.DialogueVO.mariposa_sad_short_sigh},
        {"Mariposa_Sigh4", AudioEvents.DialogueVO.mariposa_sad_short_sigh},
        {"Mariposa_Sorry3", AudioEvents.DialogueVO.mariposa_sad_short_sorry},
        {"Mariposa_ThankYou1", AudioEvents.DialogueVO.mariposa_happy_medium_thank_you},
        {"Mariposa_UpAndRunning1", AudioEvents.DialogueVO.mariposa_happy_medium_up_and_running},
        {"Mariposa_Urgh3", AudioEvents.DialogueVO.mariposa_sad_short_urgh},
        {"Mariposa_Urgh4", AudioEvents.DialogueVO.mariposa_sad_short_urgh},
        {"Mariposa_Well2", AudioEvents.DialogueVO.mariposa_neutral_short_well},
        {"Mariposa_Well3", AudioEvents.DialogueVO.mariposa_neutral_short_well},
        {"Mariposa_Well4", AudioEvents.DialogueVO.mariposa_neutral_short_well},
        {"Mariposa_WhyNoWork1", AudioEvents.DialogueVO.mariposa_sad_medium_why_no_work},
        {"Mariposa_Woah1", AudioEvents.DialogueVO.mariposa_surprised_short_woah},
        {"Mariposa_Woah3", AudioEvents.DialogueVO.mariposa_surprised_short_woah},
        {"Mariposa_Woah4", AudioEvents.DialogueVO.mariposa_surprised_short_woah},
        {"Mariposa_Yeah1", AudioEvents.DialogueVO.mariposa_happy_short_yeah},
        {"Mariposa_Yeah3", AudioEvents.DialogueVO.mariposa_happy_short_yeah},
        {"Mariposa_YouOk1", AudioEvents.DialogueVO.mariposa_surprised_short_you_ok},

        {"Unnamed_AnotherWay3", AudioEvents.DialogueVO.unnamed_neutral_medium_another_way},
        {"Unnamed_ComeOn1", AudioEvents.DialogueVO.unnamed_neutral_short_come_on},
        {"Unnamed_Cough1", AudioEvents.DialogueVO.unnamed_sfx_cough},
        {"Unnamed_Cough2", AudioEvents.DialogueVO.unnamed_sfx_cough},
        {"Unnamed_DamnIt1", AudioEvents.DialogueVO.unnamed_surprised_short_damn_it},
        {"Unnamed_DamnIt5", AudioEvents.DialogueVO.unnamed_surprised_short_damn_it},
        {"Unnamed_DryLaugh1", AudioEvents.DialogueVO.unnamed_happy_short_dry_laugh},
        {"Unnamed_DryLaugh3", AudioEvents.DialogueVO.unnamed_happy_short_dry_laugh},
        {"Unnamed_Hey1", AudioEvents.DialogueVO.unnamed_neutral_short_hey},
        {"Unnamed_Hey3", AudioEvents.DialogueVO.unnamed_neutral_short_hey},
        {"Unnamed_HowBoutThis3", AudioEvents.DialogueVO.unnamed_neutral_friendship_how_bout_this},
        {"Unnamed_ImSorry1", AudioEvents.DialogueVO.unnamed_sad_short_im_sorry},
        {"Unnamed_Mari2", AudioEvents.DialogueVO.unnamed_sad_medium_mari},
        {"Unnamed_Mari3", AudioEvents.DialogueVO.unnamed_sad_medium_mari},
        {"Unnamed_Mari4", AudioEvents.DialogueVO.unnamed_sad_medium_mari},
        {"Unnamed_Mari5", AudioEvents.DialogueVO.unnamed_sad_medium_mari},
        {"Unnamed_Mariposa2", AudioEvents.DialogueVO.unnamed_sad_medium_mariposa},
        {"Unnamed_Mariposa3", AudioEvents.DialogueVO.unnamed_sad_medium_mariposa},
        {"Unnamed_Maybe1", AudioEvents.DialogueVO.unnamed_happy_short_maybe},
        {"Unnamed_Maybe4", AudioEvents.DialogueVO.unnamed_happy_short_maybe},
        {"Unnamed_MaybeYoureRight1", AudioEvents.DialogueVO.unnamed_happy_medium_maybe_youre_right},
        {"Unnamed_MinorSetBack8", AudioEvents.DialogueVO.unnamed_neutral_medium_minor_setback},
        {"Unnamed_No3", AudioEvents.DialogueVO.mariposa_sad_short_no},
        {"Unnamed_NoTimeToWaste1", AudioEvents.DialogueVO.unnamed_neutral_medium_no_time_to_waste},
        {"Unnamed_NoTimeToWaste4", AudioEvents.DialogueVO.unnamed_neutral_medium_no_time_to_waste},
        {"Unnamed_NoTimeToWaste5", AudioEvents.DialogueVO.unnamed_neutral_medium_no_time_to_waste},
        {"Unnamed_NotListening3", AudioEvents.DialogueVO.unnamed_neutral_short_not_listening},
        {"Unnamed_NotTheTime4", AudioEvents.DialogueVO.unnamed_neutral_short_not_the_time},
        {"Unnamed_NowhereToRun1", AudioEvents.DialogueVO.unnamed_sad_medium_nowhere_to_run},
        {"Unnamed_Pain1", AudioEvents.DialogueVO.unnamed_sfx_pain},
        {"Unnamed_Pain5", AudioEvents.DialogueVO.unnamed_sfx_pain},
        {"Unnamed_RightAllAlong2", AudioEvents.DialogueVO.unnamed_sad_medium_right_all_along},
        {"Unnamed_Sigh1", AudioEvents.DialogueVO.unnamed_sad_short_sigh},
        {"Unnamed_Sigh2", AudioEvents.DialogueVO.unnamed_sad_short_sigh},
        {"Unnamed_Sigh3", AudioEvents.DialogueVO.unnamed_sad_short_sigh},
        {"Unnamed_Sigh4", AudioEvents.DialogueVO.unnamed_sad_short_sigh},
        {"Unnamed_SomethingsWrong1", AudioEvents.DialogueVO.unnamed_surprised_medium_somethings_wrong},
        {"Unnamed_StruckANerve4", AudioEvents.DialogueVO.unnamed_neutral_medium_struct_a_nerve},
        {"Unnamed_Tch1", AudioEvents.DialogueVO.unnamed_neutral_short_tch},
        {"Unnamed_Tch2", AudioEvents.DialogueVO.unnamed_neutral_short_tch},
        {"Unnamed_TellMeAboutIt3", AudioEvents.DialogueVO.unnamed_neutral_friendship_tell_me_about_it},
        {"Unnamed_TheresNoPoint1", AudioEvents.DialogueVO.unnamed_sad_medium_theres_no_point},
        {"Unnamed_TheresNoPoint2", AudioEvents.DialogueVO.unnamed_sad_medium_theres_no_point},
        {"Unnamed_TheseAutomatons4", AudioEvents.DialogueVO.unnamed_surprised_medium_these_automatons},
        {"Unnamed_ThisIsnt1", AudioEvents.DialogueVO.unnamed_surprised_short_this_isnt},
        {"Unnamed_ThisIsntRight3", AudioEvents.DialogueVO.unnamed_surprised_medium_this_isnt_right},
        {"Unnamed_TimeToMoveOn4", AudioEvents.DialogueVO.unnamed_neutral_medium_time_to_move_on},
        {"Unnamed_TimeToMoveOn5", AudioEvents.DialogueVO.unnamed_neutral_medium_time_to_move_on},
        {"Unnamed_Ugh1", AudioEvents.DialogueVO.unnamed_sad_short_ugh},
        {"Unnamed_Ugh3", AudioEvents.DialogueVO.unnamed_sad_short_ugh},
        {"Unnamed_Ugh5", AudioEvents.DialogueVO.unnamed_sad_short_ugh},
        {"Unnamed_WhatIf1", AudioEvents.DialogueVO.unnamed_neutral_friendship_what_if},
        {"Unnamed_WhosThere1", AudioEvents.DialogueVO.unnamed_surprised_medium_whos_there},
        {"Unnamed_YouThere4", AudioEvents.DialogueVO.unnamed_neutral_some_happy_you_there},
        {"Unnamed_YouThere7", AudioEvents.DialogueVO.unnamed_neutral_some_happy_you_there},
        {"Unnamed_YoureRidiculous4", AudioEvents.DialogueVO.unnamed_happy_medium_youre_ridiculous},
        {"Unnamed_YoureTellingMe3", AudioEvents.DialogueVO.unnamed_neutral_medium_youre_telling_me},

        {"Justin_NPC_Laugh1", AudioEvents.DialogueVO.justin_npc_happy_laugh},
        {"Justin_NPC_Laugh2", AudioEvents.DialogueVO.justin_npc_happy_laugh},
        {"Justin_NPC_Laugh7", AudioEvents.DialogueVO.justin_npc_happy_laugh},
        {"Justin_NPC_Okay1", AudioEvents.DialogueVO.justin_npc_sad_ok},

        {"Luke_NPC_AppreciateIt2", AudioEvents.DialogueVO.luke_npc_happy_appreciate_it},
        {"Luke_NPC_Dont1", AudioEvents.DialogueVO.luke_npc_sad_dont},
        {"Luke_NPC_Giggle-Laugh2", AudioEvents.DialogueVO.ruby_npc_happy_giggle_laugh},
        {"Luke_NPC_Hey1", AudioEvents.DialogueVO.luke_npc_neutral_hey},
        {"Luke_NPC_Hey2", AudioEvents.DialogueVO.luke_npc_neutral_hey},
        {"Luke_NPC_HiThere3", AudioEvents.DialogueVO.ruby_npc_greetings_hi_there},
        {"Luke_NPC_IThink1", AudioEvents.DialogueVO.luke_npc_neutral_i_think},
        {"Luke_NPC_IThink3", AudioEvents.DialogueVO.luke_npc_neutral_i_think},
        {"Luke_NPC_LetsThinkAboutThis1", AudioEvents.DialogueVO.luke_npc_neutral_lets_think_about_this},
        {"Luke_NPC_Sigh1", AudioEvents.DialogueVO.luke_npc_sad_sigh},
        {"Luke_NPC_Sigh2", AudioEvents.DialogueVO.luke_npc_sad_sigh},
        {"Luke_NPC_Sigh3", AudioEvents.DialogueVO.luke_npc_sad_sigh},
        {"Luke_NPC_ThanksMariposa3", AudioEvents.DialogueVO.luke_npc_happy_thanks_mariposa},
        {"Luke_NPC_Well3", AudioEvents.DialogueVO.luke_npc_neutral_well},
        {"Luke_NPC_Yeah2", AudioEvents.DialogueVO.luke_npc_happy_yeah},
        {"Luke_Robot_NPC_Alright2", AudioEvents.DialogueVO.luke_npc_robot_alright},
        {"Luke_Robot_NPC_Aww", AudioEvents.DialogueVO.luke_npc_robot_aww},
        {"Luke_Robot_NPC_Woah2", AudioEvents.DialogueVO.luke_npc_robot_woah},

        {"Regine_NPC_Hm1", AudioEvents.DialogueVO.regine_npc_neutral_hm},
        {"Regine_NPC_Hm3", AudioEvents.DialogueVO.regine_npc_neutral_hm},
        {"Regine_NPC_Hm4", AudioEvents.DialogueVO.regine_npc_neutral_hm},
        {"Regine_NPC_Hm7", AudioEvents.DialogueVO.regine_npc_neutral_hm},
        {"Regine_NPC_Honestly1", AudioEvents.DialogueVO.regine_npc_neutral_honestly},
        {"Regine_NPC_IMean2", AudioEvents.DialogueVO.regine_npc_neutral_i_mean},
        {"Regine_NPC_IMean3", AudioEvents.DialogueVO.regine_npc_neutral_i_mean},
        {"Regine_NPC_IThink1", AudioEvents.DialogueVO.regine_npc_neutral_i_think},
        {"Regine_NPC_Mari2", AudioEvents.DialogueVO.regine_npc_greetings_mari},
        {"Regine_NPC_OhHey1", AudioEvents.DialogueVO.regine_npc_greetings_oh_hey},
        {"Regine_NPC_Okay1", AudioEvents.DialogueVO.regine_npc_sad_okay},
        {"Regine_NPC_Sigh2", AudioEvents.DialogueVO.regine_npc_sad_sigh},
        {"Regine_NPC_Sigh3", AudioEvents.DialogueVO.regine_npc_sad_sigh},
        {"Regine_NPC_Yeah3", AudioEvents.DialogueVO.regine_npc_happy_yeah},

        {"Ruby_NPC_Giggle-Laugh1", AudioEvents.DialogueVO.ruby_npc_happy_giggle_laugh},
        {"Ruby_NPC_Giggle-Laugh7", AudioEvents.DialogueVO.ruby_npc_happy_giggle_laugh},
        {"Ruby_NPC_HiThere2", AudioEvents.DialogueVO.ruby_npc_greetings_hi_there},
        {"Ruby_NPC_Hm2", AudioEvents.DialogueVO.ruby_npc_neutral_hm},
        {"Ruby_NPC_Mariposa4", AudioEvents.DialogueVO.ruby_npc_greetings_mariposa},
        {"Ruby_NPC_Woah5", AudioEvents.DialogueVO.ruby_npc_surprised_woah},
        {"Ruby_NPC_YouOkay1", AudioEvents.DialogueVO.ruby_npc_surprised_you_okay},

        {"radio-clip3-novoices", AudioEvents.SFX.radio_static_no_voices},
    };

    private void Start()
    {
        playingDialogueVOInstance = new List<EventInstance>();
        playingDialogueVOEvent = new List<EventReference>();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Invalid audio events:");
        foreach (string audio in invalidAudioEvents) Debug.Log($"{audio}");
    }

    List<string> invalidAudioEvents = new List<string>();
    public void PlayDialogueAudioEffect(string sound)
    {
        bool found = dialogueAudioEvents.TryGetValue(sound, out EventReference eventRef);

        if (!found || eventRef.Equals(default) || eventRef.IsNull)
        {
            invalidAudioEvents.Add(sound);
            Debug.LogWarning($"The dialogue audio event '{sound}' is invalid! Ignoring...");
            return;
        }

        AddDialogueVO(eventRef);
    }

    public void StopAllDialogueAudioEffects(FMOD.Studio.STOP_MODE stopMode)
    {
        foreach (EventInstance vo in playingDialogueVOInstance)
        {
            AudioManager.StopEventInstance(vo, stopMode);
        }
        playingDialogueVOInstance.Clear();
        playingDialogueVOEvent.Clear();
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
        playingDialogueVOEvent.Add(vo);
        playingDialogueVOInstance.Add(voInstance);
    }
}
