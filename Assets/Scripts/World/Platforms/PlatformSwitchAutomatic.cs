using UnityEngine;
using FMODUnity;
public class PlatformSwitchAutomatic : Switch
{
    [SerializeField] private AutomaticMovingPlatform platform;
    [SerializeField] private SpriteRenderer sr;

    protected override void Start()
    {
        base.Start();
        platform.state = SwitchToggled ? MovingPlatform.PlatformState.Moving : MovingPlatform.PlatformState.Stopped;
    }

    public override void TriggerSwitch()
    {
        if (SwitchToggled) return;
        SwitchToggled = true;
        sr.flipX = true;
        platform.state = MovingPlatform.PlatformState.Moving;
        RuntimeManager.PlayOneShot(AudioEvents.SFX.lever_pull);
    }
}
