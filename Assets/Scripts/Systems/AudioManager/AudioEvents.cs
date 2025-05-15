public static class AudioEvents
{
    public enum Music
    {
        NONE,
        s0Tutorial_theme_mariposa,
        s0Tutorial_theme_unnamed,
        s1Downtown_theme_mariposa,
        s1Downtown_theme_unnamed
    }

    public static string GetPath(this Music musicEvent)
    {
        return musicEvent switch
        {
            Music.s0Tutorial_theme_mariposa => "event:/music/s0_subway_tutorial/theme_mariposa",
            Music.s0Tutorial_theme_unnamed => "event:/music/s0_subway_tutorial/theme_unnamed",
            Music.s1Downtown_theme_mariposa => "event:/music/s1_downtown_city1/theme_mariposa",
            Music.s1Downtown_theme_unnamed => null,
            _ => null
        };
    }

    public enum Ambience
    {
        NONE,
        s0Tutorial_mariposa,
        s0Tutorial_unnamed,
        s1Downtown_mariposa,
        s1Downtown_unnamed
    }

    public static string GetPath(this Ambience ambienceEvent)
    {
        return ambienceEvent switch
        {
            Ambience.s0Tutorial_mariposa => "event:/ambience/s0_subway_tutorial/mariposa",
            Ambience.s0Tutorial_unnamed => "event:/ambience/s0_subway_tutorial/unnamed",
            Ambience.s1Downtown_mariposa => "event:/ambience/s1_downtown_city1/mariposa",
            Ambience.s1Downtown_unnamed => "event:/ambience/s1_downtown_city1/unnamed",
            _ => null
        };
    }

    public enum SFX
    {
        NONE,
        item_pickup,
        bee_pickup,
        player_footstep,
        player_landing
    }

    public static string GetPath(this SFX sfxEvent)
    {
        return sfxEvent switch
        {
            SFX.item_pickup => "event:/sfx/item/pickup",
            SFX.bee_pickup => "event:/sfx/player/bee/pickup_short",
            SFX.player_footstep => "event:/sfx/player/footstep",
            SFX.player_landing => "event:/sfx/player/land",
            _ => null
        };
    }
}