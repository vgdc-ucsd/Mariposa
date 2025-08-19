using FMOD.Studio;
using FMODUnity;
using UnityEngine;

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

    public static EventInstance CreateEventInstance(EventReference eventReference)
    {
        if (eventReference.IsNull || eventReference.Equals(default(EventReference)))
        {
            Debug.LogError("Tried to create an event instance from an invalid event");
            return default;
        }

        EventInstance newEventInstance = RuntimeManager.CreateInstance(eventReference);
        if (!newEventInstance.isValid())
        {
            Debug.LogError($"Failed to create a valid event instance for {eventReference.Path}");
            return default;
        }

        return newEventInstance;
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
    public EventReference s4Hometown_mariposa;
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
    public EventReference unnamed_grapple_use;
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

    [Header("Puzzle")]
    public EventReference puzzle_complete_mariposa;
    public EventReference puzzle_complete_unnamed;
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
    public EventReference mariposa_neutral_short_hmm;
    public EventReference mariposa_happy_short_alright;
    public EventReference mariposa_happy_medium_i_promise;
    public EventReference mariposa_happy_short_giggle;
    public EventReference mariposa_happy_medium_thank_you;
    public EventReference mariposa_happy_short_ready_to_go;
    public EventReference mariposa_happy_medium_glad_could_help;
    public EventReference mariposa_sad_short_sigh;
    public EventReference mariposa_surprised_short_woah;

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

    [Header("Unnamed")]
    public EventReference unnamed_sfx_cough;
    public EventReference unnamed_neutral_time_to_move_on;
    public EventReference unnamed_sad_short_ugh;

    [Header("RegineNPC")]
    public EventReference regine_npc_sad_okay;
    public EventReference regine_npc_neutral_hm;
    public EventReference regine_npc_neutral_i_mean;
    public EventReference regine_npc_neutral_honestly;
    public EventReference regine_npc_happy_yeah;
    public EventReference regine_npc_sad_sigh;
    public EventReference regine_npc_neutral_i_think;
    public EventReference regine_npc_greetings_mari;

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
