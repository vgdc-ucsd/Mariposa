using UnityEngine;
using System.Collections.Generic;

public class FanSwitch : Switch
{
    [SerializeField]
    private List<VelocityField> fanFields;

    public override void TriggerSwitch()
    {
        foreach (VelocityField field in fanFields)
        {
            field.OnFieldToggle();
        }
    }
}