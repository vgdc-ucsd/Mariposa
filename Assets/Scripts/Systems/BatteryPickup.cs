using UnityEngine;
using System.Collections;

public class BatteryPickup : MonoBehaviour
{
    [SerializeField] private BatteryItem batteryItem;
    [SerializeField] private float checkInterval = 1f;
    [SerializeField] private InventoryType targetInventory = InventoryType.Mariposa;
    
    private Vector3 spawnPosition;
    private SpriteRenderer sr;
    private Collider2D col;
    
    private void Awake()
    {
        spawnPosition = transform.position;
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(InventoryManager.Instance != null && batteryItem != null)
            {
                InventoryManager.Instance.AddItem(targetInventory, batteryItem);
            }
            else
            {
                Debug.LogError("InventoryManager.Instance or batteryItem is null!");
            }
            sr.enabled = false;
            col.enabled = false;
            
            StartCoroutine(RespawnWhenBatteryCountZero());
        }
    }
    
    private IEnumerator RespawnWhenBatteryCountZero()
    {
        while(InventoryManager.Instance.GetItemCount(targetInventory, batteryItem) > 0)
        {
            yield return new WaitForSeconds(checkInterval);
        }
        transform.position = spawnPosition;
        sr.enabled = true;
        col.enabled = true;
    }
}