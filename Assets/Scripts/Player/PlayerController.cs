using System.Collections.Generic;
using UnityEngine;

public enum PlayerInput
{
    MoveLeft, MoveRight, JumpPress, AbilityPress, AbilityRelease
}


public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    private Dictionary<PlayerInput, bool> currentInputs = new()
    {
        { PlayerInput.MoveLeft, false },
        { PlayerInput.MoveRight, false },
        { PlayerInput.JumpPress, false },
        { PlayerInput.AbilityPress, false },
        { PlayerInput.AbilityRelease, false },
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Update()
    {
        // temp input reading until entire input system gets set up
        if (Input.GetKeyDown(KeyCode.Space)) SendInput(PlayerInput.JumpPress);
        if (Input.GetKeyDown(KeyCode.LeftShift)) SendInput(PlayerInput.AbilityPress);
        if (Input.GetKeyUp(KeyCode.LeftShift)) SendInput(PlayerInput.AbilityRelease);
        if (Input.GetKey(KeyCode.A)) SendInput(PlayerInput.MoveLeft);
        if (Input.GetKey(KeyCode.D)) SendInput(PlayerInput.MoveRight);
    }

    public void SendInput(PlayerInput input)
    {
        currentInputs[input] = true;
    }

    private void FixedUpdate()
    {
        // do actions based on the active inputs this frame

        // not moving or moving two directions at once
        if (currentInputs[PlayerInput.MoveLeft] == currentInputs[PlayerInput.MoveRight])
        {
            Player.ActivePlayer.Movement.MoveTowards(0);
        }
        else
        {
            int dir = currentInputs[PlayerInput.MoveLeft] ? -1 : 1;
            Player.ActivePlayer.Movement.MoveTowards(dir);
        }

        if (currentInputs[PlayerInput.JumpPress])
        {
            Player.ActivePlayer.Ability.JumpInputDown();
            Player.ActivePlayer.Movement.Jump();
        }
        if (currentInputs[PlayerInput.AbilityPress])
        {
            Player.ActivePlayer.Ability.AbilityInputDown();
        }
        if (currentInputs[PlayerInput.AbilityRelease])
        {
            Player.ActivePlayer.Ability.AbilityInputUp();
        }


        foreach (var key in new List<PlayerInput>(currentInputs.Keys))
        {
            // reset all inputs to false until next frame
            currentInputs[key] = false;
        }
    }

}