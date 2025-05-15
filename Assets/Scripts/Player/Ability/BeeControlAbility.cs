using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class BeeControlAbility : MonoBehaviour, IAbility
{
    public Bee BeeRef;
    EventInstance BeeFlap;


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (Bee.Instance.Movement.CurrentBehavior is not Follow) { RuntimeManager.PlayOneShot("event:/sfx/player/bee/recall"); }
            BeeRef.StartFollow();
        }
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
            Debug.LogError("Bee is not assigned");
            return;
        }
        if (!BeeRef.IsControlled)
        {
            BeeRef.ToggleControl(true);
            RuntimeManager.PlayOneShot("event:/sfx/player/bee/deploy");
            BeeFlap.start();
        }
        else
        {
            BeeRef.ToggleControl(false);
            //RuntimeManager.PlayOneShot("event:/sfx/player/bee/recall");
            // maybe should be swap sfx for perspective swapping for player
            BeeFlap.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void Start()
    {
        BeeFlap = RuntimeManager.CreateInstance("event:/sfx/player/bee/flap");
    }
}