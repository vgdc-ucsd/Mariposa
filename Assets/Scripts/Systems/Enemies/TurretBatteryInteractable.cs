using UnityEngine;

public class TurretBatteryInteractable : Interactable
{
    [SerializeField] private DowntownTurret parentTurret;
    [SerializeField] private TurretBattery batteryClass;
    [SerializeField] private ItemData batteryItemData;

    protected override void Start()
    {
        //                          no, this is not a typo
        parentTurret = this.transform.parent.parent.gameObject.GetComponent<DowntownTurret>();
        if (parentTurret == null)
        {
            Debug.Log("parentTurret not found; sadge");
        }
    }

    public override void OnInteract(IControllable controllable)
    {
        if (parentTurret == null)
        {
            Debug.LogWarning("no parent turret; function stopped");
            return;
        }
        parentTurret.RemoveBattery();
        InventoryManager.Instance.GetInventory().AddItem(batteryItemData);

        Destroy(batteryClass.gameObject);
    }
}
