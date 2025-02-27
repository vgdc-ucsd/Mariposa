using UnityEngine;

public class BeeControlAbility : MonoBehaviour, IAbility
{
    public BeeMovement BeeRef;
    public bool controllingBee = false;


    public void AbilityInputDown()
    {
        ToggleBeeControl();
    }

    public void Initialize()
    {
        controllingBee = false;
    }

    private void ToggleBeeControl()
    {
        if (BeeRef == null)
        {
            Debug.LogError("Bee is not assigned");
            return;
        }
        if (!controllingBee)
        {
            PlayerController.Instance.StartControlling(BeeRef);
            controllingBee = true;
        }
        else
        {
            PlayerController.Instance.StartControlling(Player.ActivePlayer.Movement);
            controllingBee = false;
        }

    }
}