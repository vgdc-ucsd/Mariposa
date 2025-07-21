using System;
using UnityEngine;

public class CranePressurePlate : PressurePlate
{
    [SerializeField] Crane crane;

    public virtual void NotEnoughBatteries() {}
    public virtual void EnoughBatteries() {}
    protected override void OnPress()
    {
        if (numBatteries < requiredBatteries)
        {
            NotEnoughBatteries();
            return;
        }
        crane.TriggerCrane();
        EnoughBatteries();
    }

    protected override void OnRelease()
    {
        crane.ReturnCrane();
    }
}