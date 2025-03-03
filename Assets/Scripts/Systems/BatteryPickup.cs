using UnityEngine;
using System.Collections;

public class BatteryPickup : ItemPickup
{
    private Vector3 spawnPosition;
    private SpriteRenderer sr;
    private Collider2D col;
    private float pickupDelay = 1;
    
    protected override void Awake()
    {
        base.Awake();
        spawnPosition = transform.position;
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }
    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (InventoryManager.Instance != null && item != null)
            {
                InventoryManager.Instance.AddItem(targetInventory, item);
            }
            else
            {
                Debug.LogError("InventoryManager.Instance or item is null!");
            }
            sr.enabled = false;
            col.enabled = false;
            
            StartCoroutine(RespawnWhenBatteryCountZero());
        }
    }
    
    private IEnumerator RespawnWhenBatteryCountZero()
    {
        while (InventoryManager.Instance.GetItemCount(targetInventory, item) > 0)
        {
            yield return new WaitForSeconds(pickupDelay > 0 ? pickupDelay : 1f);
        }
        transform.position = spawnPosition;
        sr.enabled = true;
        col.enabled = true;
    }
    */
}