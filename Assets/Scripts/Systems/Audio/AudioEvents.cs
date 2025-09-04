using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using System.Reflection;
using System.Text;

public class AudioEvents : MonoBehaviour
{
    private static AudioEvents _instance;

    [SerializeField] private MusicEvents _music;
    public static MusicEvents Music => _instance._music;

    [SerializeField] private SFXEvents _sfx;
    public static SFXEvents SFX => _instance._sfx;

    [SerializeField] private AmbienceEvents _ambience;
    public static AmbienceEvents Ambience => _instance._ambience;

    [SerializeField] private DialogueVOEvents _dialogueVO;
    public static DialogueVOEvents DialogueVO => _instance._dialogueVO;

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;
    }
}

[System.Serializable]
public class MusicEvents
{
    public EventReference s0Tutorial_mariposa;
    public EventReference s0Tutorial_unnamed;
    public EventReference s1Downtown_mariposa;
    public EventReference s1Downtown_unnamed;
    public EventReference s2Pier_mariposa;
    public EventReference S2Pier_unnamed;
    public EventReference s3BigRobot_unnamed;
    public EventReference s4Hometown_mariposa;
    public EventReference s4Hometown_unnamed;
    public EventReference titlescreen_title;
}

[System.Serializable]
public class AmbienceEvents
{ 
    public EventReference s0Tutorial_mariposa;
    public EventReference s0Tutorial_unnamed;
    public EventReference s1Downtown_mariposa;
    public EventReference s1Downtown_unnamed;
    public EventReference s4Hometown_unnamed;
}

[System.Serializable]
public class SFXEvents
{
    [Header("Mariposa")]
    public EventReference mariposa_jump;
    public EventReference mariposa_recall;
    public EventReference mariposa_send_out;
    public EventReference mariposa_hum_motif;

    [Header("Unnamed")]
    public EventReference unnamed_cough;
    public EventReference unnamed_jump;
    public EventReference unnamed_pain;
    public EventReference unnamed_grapple_impact;
    public EventReference unnamed_grapple_swing;
    public EventReference unnamed_grapple_throw;
    public EventReference unnamed_whistle_mariposa_motif;
    public EventReference unnamed_whistle_unnamed_motif;

    [Header("Player")]
    public EventReference player_footstep;
    public EventReference player_landing;
    public EventReference player_jump;
    public EventReference player_trip;

    [Header("Bee")]
    public EventReference bee_double_jump;
    public EventReference bee_flap;
    public EventReference bee_hover;
    public EventReference bee_pickup_long;
    public EventReference bee_pickup_short;
    public EventReference bee_recall;

    [Header("Squid")]
    public EventReference squid_activation;
    public EventReference squid_footstep;

    [Header("Item")]
    public EventReference item_pickup;
    public EventReference item_use;
    public EventReference keycard_spit;
    public EventReference keycard_tap;
    public EventReference radio_static;
    public EventReference radio_static_no_voices;

    [Header("Puzzle")]
    public EventReference puzzle_complete_mariposa;
    public EventReference puzzle_complete_unnamed;
    public EventReference block_pickup;
    public EventReference block_place;
    public EventReference lever_pull;

    [Header("UI")]
    public EventReference button_click;
    public EventReference dialogue_advance;
    public EventReference slider_click;

    [Header("World")]
    public EventReference earthquake_rocks;
    public EventReference earthquale_rumble_rocks;
    public EventReference metal_door_open;
    public EventReference metal_door_thud;
    public EventReference robot_noises;
    public EventReference robot_step;
    public EventReference spawnpoint_activate_mariposa;
    public EventReference spawnpoint_activate_unnamed;
    public EventReference change_battery;
    public EventReference exit_tutorial_unnamed;
    public EventReference turnstile_beep;
    public EventReference wall_jump_rock_shift;
}

[System.Serializable]
public class DialogueVOEvents {
    [Header("Mariposa")]
    public EventReference mariposa_happy_medium_glad_could_help;
    public EventReference mariposa_happy_medium_i_promise;
    public EventReference mariposa_happy_medium_in_order;
    public EventReference mariposa_happy_medium_piece_of_cake;
    public EventReference mariposa_happy_medium_see_ya_round;
    public EventReference mariposa_happy_medium_thank_you;
    public EventReference mariposa_happy_medium_up_and_running;

    public EventReference mariposa_happy_short_alright;
    public EventReference mariposa_happy_short_giggle;
    public EventReference mariposa_happy_short_it_works;
    public EventReference mariposa_happy_short_ready_to_go;
    public EventReference mariposa_happy_short_yeah;

    public EventReference mariposa_neutral_medium_how_bout_this;
    public EventReference mariposa_neutral_medium_miscalculated;
    public EventReference mariposa_neutral_medium_new_perspective;
    public EventReference mariposa_neutral_medium_one_thing;
    public EventReference mariposa_neutral_medium_the_way_around;

    public EventReference mariposa_neutral_short_hmm;
    public EventReference mariposa_neutral_short_i_mean;
    public EventReference mariposa_neutral_short_well;

    public EventReference mariposa_sad_medium_maybe_im_wrong;
    public EventReference mariposa_sad_medium_nothing_i_can_do;
    public EventReference mariposa_sad_medium_really_sorry;
    public EventReference mariposa_sad_medium_try_again;
    public EventReference mariposa_sad_medium_why_no_work;

    public EventReference mariposa_sad_short_aww;
    public EventReference mariposa_sad_short_no;
    public EventReference mariposa_sad_short_sigh;
    public EventReference mariposa_sad_short_sorry;
    public EventReference mariposa_sad_short_urgh;

    public EventReference mariposa_surprised_medium_dont_scare_me;
    public EventReference mariposa_surprised_medium_look_out;
    public EventReference mariposa_surprised_medium_that_was_close;

    public EventReference mariposa_surprised_short_hey;
    public EventReference mariposa_surprised_short_seriously;
    public EventReference mariposa_surprised_short_wait_a_second;
    public EventReference mariposa_surprised_short_woah;
    public EventReference mariposa_surprised_short_you_ok;

    [Header("Unnamed")]
    public EventReference unnamed_happy_medium_maybe_youre_right;
    public EventReference unnamed_happy_medium_sure_id_like_that;
    public EventReference unnamed_happy_medium_you_did_it;
    public EventReference unnamed_happy_medium_youre_ridiculous;

    public EventReference unnamed_happy_short_dry_laugh;
    public EventReference unnamed_happy_short_good_work;
    public EventReference unnamed_happy_short_maybe;
    public EventReference unnamed_happy_short_nice;

    public EventReference unnamed_neutral_friendship_how_bout_this;
    public EventReference unnamed_neutral_friendship_tell_me_about_it;
    public EventReference unnamed_neutral_friendship_what_if;
    public EventReference unnamed_neutral_friendship_youre_fine;

    public EventReference unnamed_neutral_medium_another_way;
    public EventReference unnamed_neutral_medium_fine_by_me;
    public EventReference unnamed_neutral_medium_minor_setback;
    public EventReference unnamed_neutral_medium_no_time_to_waste;
    public EventReference unnamed_neutral_medium_struct_a_nerve;
    public EventReference unnamed_neutral_medium_time_to_move_on;
    public EventReference unnamed_neutral_medium_youre_telling_me;

    public EventReference unnamed_neutral_short_come_on;
    public EventReference unnamed_neutral_short_fine;
    public EventReference unnamed_neutral_short_hey;
    public EventReference unnamed_neutral_short_not_listening;
    public EventReference unnamed_neutral_short_not_the_time;
    public EventReference unnamed_neutral_short_tch;
    public EventReference unnamed_neutral_short_this_again;
    public EventReference unnamed_neutral_short_useless_drivel;
    public EventReference unnamed_neutral_short_whatever;

    public EventReference unnamed_neutral_some_happy_i_suppose_so;
    public EventReference unnamed_neutral_some_happy_its_ok;
    public EventReference unnamed_neutral_some_happy_thank_you;
    public EventReference unnamed_neutral_some_happy_you_there;

    public EventReference unnamed_sad_medium_had_to_be_like_this;
    public EventReference unnamed_sad_medium_mari;
    public EventReference unnamed_sad_medium_mariposa;
    public EventReference unnamed_sad_medium_nowhere_to_run;
    public EventReference unnamed_sad_medium_pain_had_to_be_like_this;
    public EventReference unnamed_sad_medium_right_all_along;
    public EventReference unnamed_sad_medium_theres_no_point;
    public EventReference unnamed_sad_medium_wasnt_your_fault;

    public EventReference unnamed_sad_short_im_sorry;
    public EventReference unnamed_sad_short_no;
    public EventReference unnamed_sad_short_not_yet;
    public EventReference unnamed_sad_short_sigh;
    public EventReference unnamed_sad_short_ugh;

    public EventReference unnamed_surprised_medium_get_back;
    public EventReference unnamed_surprised_medium_not_a_step_closer;
    public EventReference unnamed_surprised_medium_somethings_wrong;
    public EventReference unnamed_surprised_medium_these_automatons;
    public EventReference unnamed_surprised_medium_this_is;
    public EventReference unnamed_surprised_medium_this_isnt_right;
    public EventReference unnamed_surprised_medium_whos_there;

    public EventReference unnamed_surprised_short_damn_it;
    public EventReference unnamed_surprised_short_no;
    public EventReference unnamed_surprised_short_this_isnt;

    public EventReference unnamed_sfx_cough;
    public EventReference unnamed_sfx_pain;

    [Header("Beebo")]
    public EventReference beebo_alert;
    public EventReference beebo_bee_right_back;
    public EventReference beebo_error_erro;
    public EventReference beebo_fastest_route;
    public EventReference beebo_got_mail;
    public EventReference beebo_home_safe;
    public EventReference beebo_im_back;
    public EventReference beebo_just_a_moment;
    public EventReference beebo_lets_go;
    public EventReference beebo_ms_mariposa;
    public EventReference beebo_on_my_way;
    public EventReference beebo_order_received;
    public EventReference beebo_ow;
    public EventReference beebo_processing;
    public EventReference beebo_ready_to_go;
    public EventReference beebo_ready_when_you_are;
    public EventReference beebo_rerouting;
    public EventReference beebo_right_on_schedule;
    public EventReference beebo_understood;
    public EventReference beebo_where_to_next;

    [Header("LukeNPC")]
    public EventReference luke_npc_neutral_well;
    public EventReference luke_npc_happy_appreciate_it;
    public EventReference luke_npc_sad_sigh;
    public EventReference luke_npc_neutral_i_think;
    public EventReference luke_npc_neutral_hey;
    public EventReference luke_npc_sad_dont;
    public EventReference luke_npc_happy_yeah;
    public EventReference luke_npc_robot_aww;
    public EventReference luke_npc_neutral_lets_think_about_this;
    public EventReference luke_npc_happy_thanks_mariposa;
    public EventReference luke_npc_robot_alright;
    public EventReference luke_npc_robot_woah;

    [Header("RegineNPC")]
    public EventReference regine_npc_sad_okay;
    public EventReference regine_npc_neutral_hm;
    public EventReference regine_npc_neutral_i_mean;
    public EventReference regine_npc_neutral_honestly;
    public EventReference regine_npc_happy_yeah;
    public EventReference regine_npc_sad_sigh;
    public EventReference regine_npc_neutral_i_think;
    public EventReference regine_npc_greetings_mari;
    public EventReference regine_npc_greetings_oh_hey;

    [Header("RubyNPC")]
    public EventReference ruby_npc_surprised_woah;
    public EventReference ruby_npc_greetings_hi_there;
    public EventReference ruby_npc_surprised_you_okay;
    public EventReference ruby_npc_neutral_hm;
    public EventReference ruby_npc_greetings_mariposa;
    public EventReference ruby_npc_happy_giggle_laugh;

    [Header("JustinNPC")]
    public EventReference justin_npc_sad_ok;
    public EventReference justin_npc_happy_laugh;
}
