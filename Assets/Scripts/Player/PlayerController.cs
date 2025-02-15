using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Update()
    {
        // temp input reading until entire input system gets set up
        if (Input.GetKeyDown(KeyCode.Space)) JumpInputDown();
        if (Input.GetKeyDown(KeyCode.LeftShift)) AbilityInputDown();
        if (Input.GetKeyUp(KeyCode.LeftShift)) AbilityInputUp();
    }

    public void JumpInputDown()
    {
        //Player.ActivePlayer.Movement.Jump();
        Player.ActivePlayer.Ability.JumpInputDown();
    }

    public void AbilityInputDown()
    {
        Player.ActivePlayer.Ability.AbilityInputDown();
    }

    public void AbilityInputUp()
    {
        Player.ActivePlayer.Ability.AbilityInputUp();
    }
}