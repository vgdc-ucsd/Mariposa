using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    [SerializeField] private BatteryItem batteryItem;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(InventoryManager.Instance != null && batteryItem != null)
            {
                InventoryManager.Instance.AddItem(batteryItem);
            }
            else
            {
                Debug.LogError("InventoryManager.Instance or batteryItem is null!");
            }
            Destroy(gameObject);
        }
    }
}