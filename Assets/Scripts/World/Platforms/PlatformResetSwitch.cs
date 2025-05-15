using UnityEngine;

public class PlatformResetSwitch : Switch
{
    [SerializeField] MovingPlatform[] platforms;

    public override void TriggerSwitch()
    {
        foreach (var platform in platforms)
        {
            platform.state = MovingPlatform.PlatformState.Resetting;
        }
    }
}
