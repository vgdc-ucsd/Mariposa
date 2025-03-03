using UnityEngine;
using FMODUnity;
using UnityEngine.UIElements;

public class BackgroundSound : MonoBehaviour
{
    [SerializeField]
    public string BackgroundSoundEvent;
    private FMOD.Studio.EventInstance backgroundSoundInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartSound();
    }

    [ContextMenu("Start Sound")]
    public void StartSound()
    {
        StopSound();
        backgroundSoundInstance = RuntimeManager.CreateInstance(BackgroundSoundEvent);
        backgroundSoundInstance.start();
    }

    [ContextMenu("Stop Sound")]
    public void StopSound()
    {
        backgroundSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        backgroundSoundInstance.release();
    }

    public void ChangeSound(string newSoundEvent)
    {
        StopSound();
        BackgroundSoundEvent = newSoundEvent;
        StartSound();
    }

    void OnDestroy()
    {
        StopSound();
    }

    void OnDisable()
    {
        StopSound();
    }

    void OnEnable()
    {
        StartSound();
    }
}
