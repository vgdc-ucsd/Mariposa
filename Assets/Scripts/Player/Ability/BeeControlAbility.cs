using UnityEngine;

public class BeeControlAbility : MonoBehaviour, IAbility
{
    public BeeMovement BeeRef;
    public bool controllingBee = false;


    public void AbilityInputDown()
    {
        ToggleBeeControl();
    }

    private void ToggleBeeControl()
    {
        if (!controllingBee)
        {
            PlayerController.Instance.Unsubscribe(Player.ActivePlayer.Movement);
            PlayerController.Instance.Subscribe(BeeRef);
            controllingBee = true;
        }
        else
        {
            PlayerController.Instance.Unsubscribe(BeeRef);
            PlayerController.Instance.Subscribe(Player.ActivePlayer.Movement);
            controllingBee = false;
        }

    }
}