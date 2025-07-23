using System;
using UnityEngine;

public class CranePressurePlate : PressurePlate
{
    [SerializeField] Crane crane;
    [SerializeField] private ItemData batteryItemNeeded;

    public virtual void NotEnoughBatteries() { }
    public virtual void EnoughBatteries() {}
    protected override void OnPress()
    {
        // check for batteries
        if (requiredBatteries != 0)
        {
            int numBatteriesInInventory = InventoryManager.Instance.GetInventory().TryGetItemCount(batteryItemNeeded);
            if (numBatteriesInInventory < requiredBatteries)
            {
                NotEnoughBatteries();
                return;
            }
        }

        // enough batteries
        crane.TriggerCrane();

        for (int i = 0; i < requiredBatteries; i++)
        {
            InventoryManager.Instance.GetInventory().TryConsumeItem(batteryItemNeeded);
        }
        EnoughBatteries();
    }

    protected override void OnRelease()
    {
        crane.ReturnCrane();
    }
}