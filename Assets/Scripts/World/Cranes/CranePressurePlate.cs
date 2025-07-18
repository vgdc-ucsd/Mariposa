using System;
using UnityEngine;

public class CranePressurePlate : PressurePlate
{
    [SerializeField] Crane crane;
    protected override void OnPress()
    {
        if (numBatteries < requiredBatteries) return;
        crane.TriggerCrane();
    }

    protected override void OnRelease()
    {
        crane.ReturnCrane();
    }
}