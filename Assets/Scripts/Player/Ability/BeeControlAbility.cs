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
            RuntimeManager.PlayOneShot("event:/sfx/player/bee/recall");
            BeeFlap.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void Start()
    {
        BeeFlap = RuntimeManager.CreateInstance("event:/sfx/player/bee/flap");
    }
}