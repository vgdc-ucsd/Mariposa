using UnityEngine;

public class SquidControlAbility : MonoBehaviour, IAbility
{
    [SerializeField] private SquidMovement squidMovement;
    [SerializeField] private PipeEnterTrigger pipes;
    private bool isControlled;

    public void Initialize()
    {
        PlayerController.Instance.StartControlling(squidMovement);
        isControlled = true;
    }

    public void ToggleControl(bool toggle)
    {
        isControlled = toggle;
        pipes.UpdateVisuals();
        if (toggle)
        {
            PlayerController.Instance.StartControlling(squidMovement);
        }
        else
        {
            PlayerController.Instance.StartControlling(Player.ActivePlayer.Movement);
        }
    }

    public void AbilityInputDown()
    {
        ToggleControl(!isControlled);
    }
}
