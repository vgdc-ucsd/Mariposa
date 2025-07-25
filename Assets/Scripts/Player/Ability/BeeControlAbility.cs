using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using FMOD;

public class BeeControlAbility : MonoBehaviour, IAbility
{
    public Bee BeeRef;
    EventInstance BeeFlap;
    private bool playSendOutSFX = true;

    public void RecallBee()
    {
        if (Bee.Instance.Movement.CurrentBehavior is not Follow && !Bee.Instance.IsControlled)
        {
            RuntimeManager.PlayOneShot(AudioEvents.SFX.mariposa_recall.GetPath());
            playSendOutSFX = true;
        }
        BeeRef.StartFollow();
    }

    public void AbilityInputDown()
    {
        ToggleBeeControl();
    }

    public void Initialize()
    {
        BeeRef.ToggleControl(false);
        BeeRef.StartFollow();
    }

    private void ToggleBeeControl()
    {
        if (BeeRef == null)
        {
            UnityEngine.Debug.LogError("Bee is not assigned");
            return;
        }
        if (!BeeRef.IsControlled)
        {
            BeeRef.ToggleControl(true);
            if (playSendOutSFX)
            {
                RuntimeManager.PlayOneShot(AudioEvents.SFX.mariposa_send_out.GetPath());
                playSendOutSFX = false;
            }
            if (!PlayerController.Instance.IsSquidUnlocked)
            {
                BeeFlap.start();                
            }
        }
        else
        {
            BeeRef.ToggleControl(false);
            RuntimeManager.PlayOneShot(AudioEvents.SFX.bee_recall.GetPath());
            BeeFlap.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void Start()
    {
        BeeFlap = RuntimeManager.CreateInstance(AudioEvents.SFX.bee_flap.GetPath());
    }

    public void TurnOffBeeFlap()
    {
        BeeFlap.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}