using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

public class FanSwitch : Switch
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private List<VelocityField> fanFields;

    public override void TriggerSwitch()
    {
        sr.flipX = !sr.flipX;
        foreach (VelocityField field in fanFields)
        {
            field.OnFieldToggle();
        }
        RuntimeManager.PlayOneShot(AudioEvents.SFX.lever_pull);
    }
}