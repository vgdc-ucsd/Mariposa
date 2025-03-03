using UnityEngine;

public class BeeControlAbility : MonoBehaviour, IAbility
{
    public Bee BeeRef;


    public void AbilityInputDown()
    {
        ToggleBeeControl();
    }

    public void Initialize()
    {
        BeeRef.ToggleControl(false);
    }

    private void ToggleBeeControl()
    {
        if (BeeRef == null)
        {
            Debug.LogError("Bee is not assigned");
            return;
        }
        if (!BeeRef.IsControlled)
        {
            BeeRef.ToggleControl(true);
        }
        else
        {
            BeeRef.ToggleControl(false);
        }

    }
}