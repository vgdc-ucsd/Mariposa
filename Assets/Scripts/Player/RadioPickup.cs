using System.Collections;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

/*
Add this script as a component to an item to make the item's effect and visibility in game disable upon contact with player.
Enables the item to be "picked up"
Currently the item is not added to any inventory but solely sets the object's active state to false
*/
public class RadioPickup : ItemPickup
{

    private DialogueManager manager;
    private EventInstance RadioPickupEvent;
    private bool playerDebug;

    [SerializeField] private Dialogue dialogue;

    protected override void Start()
    {
        base.Start();
        manager = DialogueManager.Instance;
        RadioPickupEvent = RuntimeManager.CreateInstance("event:/sfx/item/pickup");
        playerDebug = Settings.Instance.Debug.GetPlayerDebug();
    }

    /// <summary>
    /// Called when this item is picked up.
    /// Adds the item to the specified inventory and then destroys this pickup.
    /// </summary>
    public override void OnInteract(IControllable controllable)
    {
        base.OnInteract(controllable);
        manager.PlayDialogue(dialogue);

        RadioPickupEvent.setParameterByNameWithLabel("ItemType", "Default");
        RadioPickupEvent.start();
        RadioPickupEvent.release();
    }
}
