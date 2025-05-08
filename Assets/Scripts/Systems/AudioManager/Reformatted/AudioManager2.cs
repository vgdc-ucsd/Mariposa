using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public static class AudioManager2
{
    // only meant to handle custom logic for music and dialogue
    // also store a list of event instances currently active (how would one shots be effective
    public static FMOD.Studio.Bus masterBus { get { return RuntimeManager.GetBus("bus:/"); } }
    public static FMOD.Studio.Bus ambienceBus { get { return RuntimeManager.GetBus("bus:/"); } }
    public static FMOD.Studio.Bus dialogueBus { get { return RuntimeManager.GetBus("bus:/"); } }
    public static FMOD.Studio.Bus musicBus { get { return RuntimeManager.GetBus("bus:/"); } }
    public static FMOD.Studio.Bus sfxBus { get { return RuntimeManager.GetBus("bus:/"); } }

    // add ability to attach eventReference to control to show available controls
}

public static class AudioList
{
    // TODO: Enumerators should be kept up-to-date with the latest audio events
    public enum Audio_Ambience
    {
        // stage 0 - tutorial
        s0_tutorial_mariposa,
        s0_tutorial_unnamed,

        // stage 1 - city1
        s1_city1_mariposa,
        s1_city1_unnamed
    }

    // only meant for dialogue strictly played in a text box, background / one-shot dialogue is under SFX (implementation may changed)
    // will probably have segment indexes and maybe sentence indexes?
    public enum Audio_Dialogue
    {
        // stage 0 - tutorial
        s0_tutorial_mariposa,
        s0_tutorial_unnamed,

        // stage 1 - city1
        s1_city1_mariposa,
        s1_city1_unnamed
    }

    public enum Audio_Music
    {
        // Title Screen
        title,

        // stage 0 - tutorial
        s0_tutorial_mariposa,
        s0_tutorial_unnamed,

        // stage 1 - city1
        s1_city1_mariposa,
        s1_city1_unnamed
    }

    public enum Audio_SFX
    {
        // TODO: need to distinguish between 1-shots, loops, and special sfx w/ parameters
        // i may have to create objects and pass those objects to wherever its being used
        test_sfx
    }

    // TODO: Add new audio events to corresponding dictionary
    private static Dictionary<Audio_Ambience, string> ambienceAudioPaths = new Dictionary<Audio_Ambience, string>
    {
        {Audio_Ambience.s0_tutorial_mariposa, "event:/ambience/s0_tutorial/mariposa"},
        {Audio_Ambience.s0_tutorial_unnamed, "event:/ambience/s0_tutorial/unnamed"},
        {Audio_Ambience.s1_city1_mariposa, "event:/ambience/s1_city1/mariposa"},
        {Audio_Ambience.s1_city1_unnamed, "event:/ambience/s1_city1/unnamed"}
    };

    private static Dictionary<Audio_Dialogue, string> dialogueAudioPaths = new Dictionary<Audio_Dialogue, string>
    {
        {Audio_Dialogue.s0_tutorial_mariposa, "event:/dialogue/s0_tutorial/mariposa"},
        {Audio_Dialogue.s0_tutorial_unnamed, "event:/dialogue/s0_tutorial/unnamed"},
        {Audio_Dialogue.s1_city1_mariposa, "event:/dialogue/s1_city1/mariposa"},
        {Audio_Dialogue.s1_city1_unnamed, "event:/dialogue/s1_city1/unnamed"}
    };

    private static Dictionary<Audio_Music, string> musicAudioPaths = new Dictionary<Audio_Music, string>
    {
        {Audio_Music.s0_tutorial_mariposa, "event:/music/s0_tutorial/mariposa"},
        {Audio_Music.s0_tutorial_unnamed, "event:/music/s0_tutorial/unnamed"},
        {Audio_Music.s1_city1_mariposa, "event:/music/s1_city1/mariposa"},
        {Audio_Music.s1_city1_unnamed, "event:/music/s1_city1/unnamed"}
    };

    private static Dictionary<Audio_SFX, string> sfxAudioPaths = new Dictionary<Audio_SFX, string>
    {
        {Audio_SFX.test_sfx, "event:/sfx/item/keycard/spit"}
    };
}