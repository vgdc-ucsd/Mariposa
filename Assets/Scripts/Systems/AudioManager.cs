using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private EventInstance backgroundMusicInstance;
    private EventInstance ambienceInstance;

    // TODO: Implement dialogue indexing (parameter in fmod controls what dialogue line plays next)
    // TODO: How the heck do I implement the different types of SFX? (see sfx section)
    // TODO: Implement global parameter changes or a system to change local parameters of music / ambience

    public void StartMusic(Audio_Music music)
    {
        backgroundMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        backgroundMusicInstance = RuntimeManager.CreateInstance(getMusicPath(music));
        backgroundMusicInstance.start();
    }

    public void StopMusic(FMOD.Studio.STOP_MODE stopMode)
    {
        backgroundMusicInstance.stop(stopMode);
    }

    public void StartAmbience(Audio_Ambience ambience)
    {
        ambienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        ambienceInstance = RuntimeManager.CreateInstance(getAmbiencePath(ambience));
        ambienceInstance.start();
    }

    public void StopAmbience(FMOD.Studio.STOP_MODE stopMode)
    {
        ambienceInstance.stop(stopMode);
    }

    public enum Audio_Ambience
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

    // only meant for dialogue strictly played in a text box, background / one-shot dialogue is under SFX (implementation may changed)
    public enum Audio_Dialogue
    {
        // stage 0 - tutorial
        s0_tutorial_mariposa,
        s0_tutorial_unnamed,

        // stage 1 - city1
        s1_city1_mariposa,
        s1_city1_unnamed
    }

    private string getAmbiencePath(Audio_Ambience ambience)
    {
        switch (ambience)
        {
            case Audio_Ambience.s0_tutorial_mariposa:
                return "event:/ambience/s0_tutorial/mariposa";

            case Audio_Ambience.s0_tutorial_unnamed:
                return "event:/ambience/s0_tutorial/unnamed";

            case Audio_Ambience.s1_city1_mariposa:
                return "event:/ambience/s1_city1/mariposa";

            case Audio_Ambience.s1_city1_unnamed:
                return "event:/ambience/s1_city1/unnamed";

            default:
                Debug.LogError("No ambience path found!");
                return null;
        }
    }

    private string getMusicPath(Audio_Music music)
    {
        switch (music)
        {
            case Audio_Music.title:
                return "event:/music/title/theme";

            case Audio_Music.s0_tutorial_mariposa:
                return "event:/music/s0_tutorial/mariposa";

            case Audio_Music.s0_tutorial_unnamed:
                return "event:/music/s0_tutorial/unnamed";

            case Audio_Music.s1_city1_mariposa:
                return "event:/music/s1_city1/mariposa";

            case Audio_Music.s1_city1_unnamed:
                return "event:/music/s1_city1/unnamed";

            default:
                Debug.LogError("No music path found!");
                return null;
        }
    }

    private string getSFXPath(Audio_SFX sfx)
    {
        switch (sfx)
        {
            case Audio_SFX.test_sfx:
                Debug.Log("No path implementation yet!");
                return "";

            default:
                Debug.LogError("No sfx path found!");
                return null;
        }
    }

    private string getDialoguePath(Audio_Dialogue dialogue)
    {
        switch (dialogue)
        {
            case Audio_Dialogue.s0_tutorial_mariposa:
                return "event:/dialogue/s0_tutorial/mariposa";

            case Audio_Dialogue.s0_tutorial_unnamed:
                return "event:/dialogue/s0_tutorial/unnamed";

            case Audio_Dialogue.s1_city1_mariposa:
                return "event:/dialogue/s1_city1/mariposa";

            case Audio_Dialogue.s1_city1_unnamed:
                return "event:/dialogue/s1_city1/unnamed";

            default:
                Debug.LogError("No dialogue path found!");
                return null;
        }
    }
}
