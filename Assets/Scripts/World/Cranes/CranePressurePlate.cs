using System;
using UnityEngine;

public class CranePressurePlate : PressurePlate
{
    [SerializeField] Crane crane;
    protected override void OnPress()
    {
        crane.TriggerCrane();
    }

    protected override void OnRelease()
    {
        crane.TriggerCrane();
    }
}