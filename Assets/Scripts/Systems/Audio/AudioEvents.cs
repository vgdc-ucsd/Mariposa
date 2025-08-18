public static class AudioEvents
{
    public enum Music
    {
        NONE,
        s0Tutorial_theme_mariposa,
        s0Tutorial_theme_unnamed,
        s1Downtown_theme_mariposa,
        s1Downtown_theme_unnamed,
        s2Pier_theme_mariposa,
        S2Pier_theme_unnamed,
        s3BigRobot_theme_unnamed,
        s4Hometown_theme_mariposa,
        s4Hometown_theme_unnamed,
        titlescreen_title_theme,
        credits_theme
    }

    public static string GetPath(this Music musicEvent)
    {
        return musicEvent switch
        {
            Music.s0Tutorial_theme_mariposa => "event:/music1/s0_subway_tutorial/theme_mariposa",
            Music.s0Tutorial_theme_unnamed => "event:/music1/s0_subway_tutorial/theme_unnamed",
            Music.s1Downtown_theme_mariposa => "event:/music1/s1_downtown_city1/theme_mariposa",
            Music.s1Downtown_theme_unnamed => "event:/music1/s1_downtown_city1/theme_unnamed",
            Music.s2Pier_theme_mariposa => "event:/music1/s2_pier/theme_mariposa",
            Music.S2Pier_theme_unnamed => "event:/music1/s2_pier/theme_unnamed",
            Music.s3BigRobot_theme_unnamed => "event:/music1/s3_industrial/theme_unnamed_BIGROBOT",
            Music.s4Hometown_theme_mariposa => "event:/music1/s4_hometown/theme_mariposa",
            Music.s4Hometown_theme_unnamed => "event:/music1/s4_hometown/theme_unnamed",
            Music.titlescreen_title_theme => "event:/music1/title_screen/title_theme",
            Music.credits_theme => "event:/music1/theme_credits",
            _ => null
        };
    }

    public static bool IsValid(this Music musicEvent)
    {
        string path = musicEvent.GetPath();
        return path != null;
    }

    public enum Ambience
    {
        NONE,
        s0Tutorial_mariposa,
        s0Tutorial_unnamed,
        s1Downtown_mariposa,
        s1Downtown_unnamed,
        s4Hometown_mariposa,
        s4Hometown_unnamed
    }

    public static string GetPath(this Ambience ambienceEvent)
    {
        return ambienceEvent switch
        {
            Ambience.s0Tutorial_mariposa => "event:/ambience/s0_subway_tutorial/mariposa",
            Ambience.s0Tutorial_unnamed => "event:/ambience/s0_subway_tutorial/unnamed",
            Ambience.s1Downtown_mariposa => "event:/ambience/s1_downtown_city1/mariposa",
            Ambience.s1Downtown_unnamed => "event:/ambience/s1_downtown_city1/unnamed",
            Ambience.s4Hometown_mariposa => "PLACEHOLDER",
            Ambience.s4Hometown_unnamed => "PLACEHOLDER",
            _ => null
        };
    }

    public enum SFX
    {
        NONE,
        item_pickup,
        bee_flap,
        bee_pickup,
        bee_double_jump,
        bee_recall,
        player_footstep,
        player_landing,
        player_jump,
        mariposa_jump,
        unnamed_jump,
        unnamed_pain,
        unnamed_grapple,
        mariposa_recall,
        mariposa_send_out,
        squid_activation,
        squid_footstep

    }

    public static string GetPath(this SFX sfxEvent)
    {
        return sfxEvent switch
        {
            SFX.item_pickup => "event:/sfx/item/pickup",
            SFX.bee_flap => "event:/sfx/player/bee/flap",
            SFX.bee_pickup => "event:/sfx/player/bee/pickup_short",
            SFX.bee_recall => "event:/sfx/player/bee/recall",
            SFX.bee_double_jump => "event:/sfx/player/bee/double_jump",
            SFX.player_footstep => "event:/sfx/player/footstep",
            SFX.player_landing => "event:/sfx/player/land",
            SFX.player_jump => "event:/sfx/player/jump",
            SFX.mariposa_jump => "event:/dialogue/mariposa/sfx/jump",
            SFX.unnamed_jump => "event:/dialogue/unnamed/sfx/jump",
            SFX.unnamed_pain => "event:/dialogue/unnamed/sfx/pain",
            SFX.unnamed_grapple => "event:/sfx/player/grapple/impact",
            SFX.mariposa_recall => "event:/dialogue/mariposa/recall/ALL",
            SFX.mariposa_send_out => "event:/dialogue/mariposa/send_out/ALL",
            SFX.squid_activation => "event:/sfx/player/squid/activation",
            SFX.squid_footstep => "event:/sfx/player/squid/footstep",
            _ => null
        };
    }

    public enum DialogueVO
    {
        NONE,
        mariposa_neutral_short_hmm,
        mariposa_happy_short_alright,
        mariposa_happy_medium_i_promise,
        mariposa_happy_short_giggle,
        mariposa_happy_medium_thank_you,
        mariposa_happy_short_ready_to_go,
        mariposa_happy_medium_glad_could_help,
        mariposa_sad_short_sigh,
        mariposa_surprised_short_woah,
        luke_npc_neutral_well,
        luke_npc_happy_appreciate_it,
        luke_npc_sad_sigh,
        luke_npc_neutral_i_think,
        luke_npc_neutral_hey,
        luke_npc_sad_dont,
        luke_npc_happy_yeah,
        luke_npc_robot_aww,
        luke_npc_neutral_lets_think_about_this,
        unnamed_sfx_cough,
        unnamed_neutral_time_to_move_on,
        unnamed_sad_short_ugh,
        regine_npc_sad_okay,
        regine_npc_neutral_hm,
        regine_npc_neutral_i_mean,
        regine_npc_neutral_honestly,
        regine_npc_happy_yeah,
        regine_npc_sad_sigh,
        regine_npc_neutral_i_think,
        regine_npc_greetings_mari,
        ruby_npc_surprised_woah,
        ruby_npc_greetings_hi_there,
        ruby_npc_surprised_you_okay,
        ruby_npc_neutral_hm,
        ruby_npc_greetings_mariposa,
        ruby_npc_happy_giggle_laugh,
        justin_npc_sad_ok,
        justin_npc_happy_laugh
    }

    public static string GetPath(this DialogueVO dialogueEvent)
    {
        return dialogueEvent switch
        {
            DialogueVO.mariposa_neutral_short_hmm => "event:/dialogue/mariposa/neutral/short/hmm",
            DialogueVO.mariposa_happy_short_alright => "event:/dialogue/mariposa/happy/short/alright",
            DialogueVO.mariposa_happy_medium_i_promise => "event:/dialogue/mariposa/happy/medium/i_promise",
            DialogueVO.mariposa_happy_short_giggle => "event:/dialogue/mariposa/happy/short/giggle",
            DialogueVO.mariposa_happy_medium_thank_you => "event:/dialogue/mariposa/happy/medium/thank_you",
            DialogueVO.mariposa_happy_short_ready_to_go => "event:/dialogue/mariposa/happy/short/ready_to_go",
            DialogueVO.mariposa_happy_medium_glad_could_help => "event:/dialogue/mariposa/happy/medium/glad_could_help",
            DialogueVO.mariposa_sad_short_sigh => "event:/dialogue/mariposa/sad/short/sigh",
            DialogueVO.mariposa_surprised_short_woah => "event:/dialogue/mariposa/surprised/short/woah",
            DialogueVO.luke_npc_neutral_well => "event:/dialogue/luke/npc_neutral/well",
            DialogueVO.luke_npc_happy_appreciate_it => "event:/dialogue/luke/npc_happy/appreciate_it",
            DialogueVO.luke_npc_sad_sigh => "event:/dialogue/luke/npc_sad/sigh",
            DialogueVO.luke_npc_neutral_i_think => "event:/dialogue/luke/npc_neutral/i_think",
            DialogueVO.luke_npc_neutral_hey => "event:/dialogue/luke/npc_neutral/hey",
            DialogueVO.luke_npc_sad_dont => "event:/dialogue/luke/npc_sad/dont",
            DialogueVO.luke_npc_happy_yeah => "event:/dialogue/luke/npc_happy/yeah",
            DialogueVO.luke_npc_robot_aww => "event:/dialogue/luke/npc_robot/aww",
            DialogueVO.luke_npc_neutral_lets_think_about_this => "event:/dialogue/luke/npc_neutral/lets_think_about_this",
            DialogueVO.unnamed_sfx_cough => "event:/dialogue/unnamed/sfx/cough",
            DialogueVO.unnamed_neutral_time_to_move_on => "event:/dialogue/unnamed/neutral/medium/time_to_move_on",
            DialogueVO.unnamed_sad_short_ugh => "event:/dialogue/unnamed/sad/short/ugh",
            DialogueVO.regine_npc_sad_okay => "event:/dialogue/regine/npc_sad/okay",
            DialogueVO.regine_npc_neutral_hm => "event:/dialogue/regine/npc_neutral/hm",
            DialogueVO.regine_npc_neutral_i_mean => "event:/dialogue/regine/npc_neutral/i_mean",
            DialogueVO.regine_npc_neutral_honestly => "event:/dialogue/regine/npc_neutral/honestly",
            DialogueVO.regine_npc_happy_yeah => "event:/dialogue/regine/npc_happy/yeah",
            DialogueVO.regine_npc_sad_sigh => "event:/dialogue/regine/npc_sad/sigh",
            DialogueVO.regine_npc_neutral_i_think => "event:/dialogue/regine/npc_neutral/i_think",
            DialogueVO.regine_npc_greetings_mari => "event:/dialogue/regine/npc_greetings/mari",
            DialogueVO.ruby_npc_surprised_woah => "event:/dialogue/ruby/npc_surprised/woah",
            DialogueVO.ruby_npc_greetings_hi_there => "event:/dialogue/ruby/npc_greetings/hi_there",
            DialogueVO.ruby_npc_surprised_you_okay => "event:/dialogue/ruby/npc_surprised/you_okay",
            DialogueVO.ruby_npc_neutral_hm => "event:/dialogue/ruby/npc_neutral/hm",
            DialogueVO.ruby_npc_greetings_mariposa => "event:/dialogue/ruby/npc_greetings/mariposa",
            DialogueVO.ruby_npc_happy_giggle_laugh => "event:/dialogue/ruby/npc_happy/giggle_laugh",
            DialogueVO.justin_npc_sad_ok => "event:/dialogue/justin/npc_sad/ok",
            DialogueVO.justin_npc_happy_laugh => "event:/dialogue/justin/npc_happy/laugh",
            _ => null
        };
    }
}