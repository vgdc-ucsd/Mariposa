using System.Collections.Generic;
using UnityEngine;

public class SwitchInteraction : Interactable
{
    public List<GameObject> playerInteractions;
    
    /// <summary>
    /// Finds every interactable object, such as buttons and levers.
    /// Items found are put into the playerInteractions list.
    /// </summary>
    void Awake()
    {
        playerInteractions = new List<GameObject>();
        playerInteractions.AddRange(GameObject.FindGameObjectsWithTag("Interactable"));
    }

    /// <summary>
    /// Finds the closest object to the player.
    /// returns null if the object is further than the player's interaction distance.
    /// </summary>
    GameObject FindClosestInteractable()
    {
        float closestDistance = Mathf.Infinity;
        
        GameObject closestInteractable = null;
        
        for (int i = 0; i < playerInteractions.Count; i++)
        {
            float dist = Vector3.Distance(playerInteractions[i].transform.position, transform.position);

            if (dist >= closestDistance) continue;
                
            closestDistance = dist;
                
            if (closestDistance <= Proximity) closestInteractable = playerInteractions[i];
        }
        
        return closestInteractable;
    }
    
    /// <summary>
    /// If the player is next to an interactable, trigger the interactable.
    /// </summary>
    public override void OnInteract()
    {
        GameObject closestInteractable = FindClosestInteractable();
        
        Debug.Log(closestInteractable);
        
        if (closestInteractable != null) closestInteractable.GetComponent<Switch>().TriggerSwitch();
    }
    
    protected override void SetProximity()
    {
        Proximity = 1f;
    }
}
