using System;
using UnityEngine;

public class PlatformSwitch : Switch
{
    [SerializeField] SendableMovingPlatform[] platforms;
    public override void TriggerSwitch()
    {
        foreach (var platform in platforms)
        {
            platform.SendPlatform();
        }
    }
}