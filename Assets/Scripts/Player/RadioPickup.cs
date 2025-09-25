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
    private EventInstance radioStatic;
    private bool playRadioStatic = true;
    private bool playerDebug;


    [SerializeField] private string dialogueName;

    protected override void Start()
    {
        base.Start();
        manager = DialogueManager.Instance;
        RadioPickupEvent = RuntimeManager.CreateInstance(AudioEvents.SFX.item_pickup);
        radioStatic = RuntimeManager.CreateInstance(AudioEvents.SFX.radio_static);
        RuntimeManager.AttachInstanceToGameObject(radioStatic, gameObject);
        playerDebug = GameManager.Instance.DebugSettings.PlayerDebugEnabled;
    }

    void Update()
    {
        if (gameObject.activeInHierarchy && playRadioStatic)
        {
            radioStatic.start();
            playRadioStatic = false;
        }
    }

    /// <summary>
    /// Called when this item is picked up.
    /// Adds the item to the specified inventory and then destroys this pickup.
    /// </summary>
    public override void OnInteract(IControllable controllable)
    {
        radioStatic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        radioStatic.release();

        base.OnInteract(controllable);
        manager.PlayDialogue(dialogueName);
    }
}
