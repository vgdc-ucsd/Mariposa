using UnityEngine;

public class KeycardDoorInteractable : Interactable
{
    [SerializeField] private KeycardDoor door;
    public override void OnInteract(IControllable controllable)
    {
        if (door.CheckForKeycard())
        {
            door.UseKeycard();
        }
    }
}
