using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public MusicEvent MusicEvent { get; private set; }
    public AmbienceEvent AmbienceEvent { get; private set; }

    // TODO: Implement dialogue indexing (parameter in fmod controls what dialogue line plays next)
    // TODO: How the heck do I implement the different types of SFX? (see sfx section)
    // TODO: Implement global parameter changes or a system to change local parameters of music / ambience

    private void Start()
    {
        // NOTE: MusicEvent and AmbienceEvent need createAudioEvent() to be called first before playing
        // used for testing -> testAudioEvents();
    }


    // START OF TEST METHODS
    private void testAudioEvents()
    {
        createAudioEvent(Audio_Music.s0_tutorial_mariposa, true);
        createAudioEvent(Audio_Ambience.s0_tutorial_mariposa, true);
        MusicEvent.DefaultControls.Play();
        AmbienceEvent.DefaultControls.Play();

        StartCoroutine(ISpamSFX(Audio_SFX.test_sfx));
    }

    private IEnumerator IPlayWaitReleaseSFX(Audio_SFX sfx)
    {
        var sfxEvent = createAudioEvent(sfx, true);
        sfxEvent.DefaultControls.Play();
        yield return new WaitForSeconds(5f);
        sfxEvent.DefaultControls.Release();
    }

    private IEnumerator ISpamSFX(Audio_SFX sfx)
    {
        while (true)
        {
            StartCoroutine(IPlayWaitReleaseSFX(sfx));
            yield return new WaitForSeconds(1f);
        }
    }
    // END OF TEST METHODS

    // START OF EXAMPLE METHODS
    // ambience and music can only have one instance of that type existing at any given point
    private void example_simpleton_call()
    {
        // command to run common commands shared by all audio events
        MusicEvent.DefaultControls.Play();
        // change the music to a new one and allow audio transitions (default is transitions on)
        MusicEvent.ChangeMusic(Audio_Music.title);

        // command to run common commands shared by all audio events
        AmbienceEvent.DefaultControls.Play();
        // change the ambience to a new one and immediately end the old one
        AmbienceEvent.ChangeAmbience(Audio_Ambience.s0_tutorial_mariposa, FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    // sfx and dialogue allow for multiple of them to exist at the same time
    private void example_non_simpleton_call()
    {
        // example sfx event with debug mode off (default is off)
        var exampleSFXEvent = createAudioEvent(Audio_SFX.test_sfx);

        // example dialogue event with debug mode enabled
        var exampleDialogueDebugEvent = createAudioEvent(Audio_Dialogue.s0_tutorial_mariposa, true);

        // command to run common commands shared by all audio events
        exampleSFXEvent.DefaultControls.Play();
        // command to run special commands that only exist for SFXs
        exampleSFXEvent.EventTest();

        // command to run common commands shared by all audio events
        exampleDialogueDebugEvent.DefaultControls.Play();
        // command to run special commands that only exist for SFXs
        exampleDialogueDebugEvent.EventTest();
    }
    // END OF EXAMPLE METHODS

    // TODO: AudioEventWrapper should inherit an interface called AudioControls to show how to use it
    // TODO: AudioManager should show that you need to create an audio event first before using with AudioControls
    // maybe create dedicated create sfx and dialogue events

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

    // List of overloads to get the correct audio event path and initialize the correct type of event

    /// <summary>
    /// Overload method to create an ambience audio event with a given ambience enumerator. Script is located in a newly created child gameObject.
    /// </summary>
    /// <param name="ambience"> A valid ambience event provided by enumerator </param>
    /// <param name="debugMode"> Creates audio event with or without debugging </param>
    /// <returns> AmbienceEvent script reference in the new gameObject </returns>
    public AmbienceEvent createAudioEvent(Audio_Ambience ambience, bool debugMode = false)
    {
        if (AmbienceEvent != null)
        {
            Debug.LogWarning("An ambience event already exists! Changing old ambience to new ambience...");
            AmbienceEvent.ChangeAmbience(ambience);
            return AmbienceEvent.Instance;
        }
        string audioEventPath = null;
        try
        {
            audioEventPath = ambienceAudioPaths[ambience];
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError("Audio path not found for " + ambience.ToString());
        }
        if (audioEventPath.Equals(""))
        {
            Debug.LogError("No audio path implementation yet for " + ambience.ToString());
            return null;
        }
        AmbienceEvent = createAudioEventGameObject<AmbienceEvent>();
        AmbienceEvent.DefaultControls.Initialize(audioEventPath, debugMode);
        return AmbienceEvent;
    }

    /// <summary>
    /// Overload method to create a dialogue audio event with a given dialogue enumerator. Script is located in a newly created child gameObject.
    /// </summary>
    /// <param name="dialogue"> A valid dialogue event provided by enumerator </param>
    /// <param name="debugMode"> Creates audio event with or without debugging </param>
    /// <returns> DialogueEvent script reference in the new gameObject </returns>
    public DialogueEvent createAudioEvent(Audio_Dialogue dialogue, bool debugMode = false)
    {
        string audioEventPath = null;
        try
        {
            audioEventPath = dialogueAudioPaths[dialogue];
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError("Audio path not found for " + dialogue.ToString());
        }
        if (audioEventPath.Equals(""))
        {
            Debug.LogError("No audio path implementation yet for " + dialogue.ToString());
            return null;
        }
        DialogueEvent output = createAudioEventGameObject<DialogueEvent>();
        output.DefaultControls.Initialize(audioEventPath, debugMode);
        output.GameObject().name = dialogue + " (" + output.DefaultControls.id + ")";
        return output;
    }

    /// <summary>
    /// Overload method to create a music audio event with a given music enumerator. Script is located in a newly created child gameObject.
    /// </summary>
    /// <param name="music"> A valid music event provided by enumerator </param>
    /// <param name="debugMode"> Creates audio event with or without debugging </param>
    /// <returns> MusicEvent script reference in the new gameObject </returns>
    public MusicEvent createAudioEvent(Audio_Music music, bool debugMode = false)
    {
        if (MusicEvent != null)
        {
            Debug.LogWarning("A music event already exists! Changing old music to new music...");
            MusicEvent.ChangeMusic(music);
            return MusicEvent;
        }
        string audioEventPath = null;
        try
        {
            audioEventPath = musicAudioPaths[music];
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError("Audio path not found for " + music.ToString());
        }
        if (audioEventPath.Equals(""))
        {
            Debug.LogError("No audio path implementation yet for " + music.ToString());
            return null;
        }
        MusicEvent = createAudioEventGameObject<MusicEvent>();
        MusicEvent.DefaultControls.Initialize(audioEventPath, debugMode);
        return MusicEvent;
    }

    /// <summary>
    /// Overload method to create a sfx audio event with a given sfx enumerator. Script is located in a newly created child gameObject.
    /// </summary>
    /// <param name="sfx"> A valid sfx event provided by enumerator </param>
    /// <param name="debugMode"> Creates audio event with or without debugging </param>
    /// <returns> SFXEvent script reference in the new gameObject </returns>
    public SFXEvent createAudioEvent(Audio_SFX sfx, bool debugMode = false)
    {
        string audioEventPath = null;
        try
        {
            audioEventPath = sfxAudioPaths[sfx];
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError("Audio path not found for " + sfx.ToString());
        }

        if (audioEventPath.Equals(""))
        {
            Debug.LogError("No audio path implementation yet for " + sfx.ToString());
            return null;
        }
        SFXEvent output = createAudioEventGameObject<SFXEvent>();
        output.DefaultControls.Initialize(audioEventPath, debugMode);
        output.GameObject().name = sfx + " (" + output.DefaultControls.id + ")";
        return output;
    }

    /// <summary>
    /// Internal helper method for createAudioEvent() to make a child gameObject of AudioManager with a given script and default name.
    /// </summary>
    /// <typeparam name="T"> Type of Audio Event </typeparam>
    /// <returns> Named child gameObject with the given event script </returns>
    private T createAudioEventGameObject<T>() where T : MonoBehaviour, IAudioEventWrapper
    {
        GameObject newObject = new GameObject(typeof(T).Name);
        newObject.AddComponent<T>();
        newObject.transform.SetParent(transform);
        return newObject.GetComponent<T>();
    }
}