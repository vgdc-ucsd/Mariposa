using System.Collections.Generic;
using UnityEngine;

public enum PlayerInput
{
    MoveLeft, MoveRight, MoveUp, MoveDown, JumpPress, AbilityPress, AbilityRelease
}


public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private List<IInputListener> listeners = new();

    private Dictionary<PlayerInput, bool> currentInputs = new()
    {
        { PlayerInput.MoveLeft, false },
        { PlayerInput.MoveRight, false },
        { PlayerInput.MoveUp, false },
        { PlayerInput.MoveDown, false },
        { PlayerInput.JumpPress, false },
        { PlayerInput.AbilityPress, false },
        { PlayerInput.AbilityRelease, false },
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void Subscribe(IInputListener listener)
    {
        listeners.Add(listener);
    }

    public void Unsubscribe(IInputListener Listener) 
    { 
        listeners.Remove(Listener); 
    }

    private void Update()
    {
        // temp input reading until entire input system gets set up
        if (Input.GetKeyDown(KeyCode.Space)) SendInput(PlayerInput.JumpPress);
        if (Input.GetKeyDown(KeyCode.LeftShift)) SendInput(PlayerInput.AbilityPress);
        if (Input.GetKeyUp(KeyCode.LeftShift)) SendInput(PlayerInput.AbilityRelease);
        if (Input.GetKey(KeyCode.A)) SendInput(PlayerInput.MoveLeft);
        if (Input.GetKey(KeyCode.D)) SendInput(PlayerInput.MoveRight);
        if (Input.GetKey(KeyCode.W)) SendInput(PlayerInput.MoveUp);
        if (Input.GetKey(KeyCode.S)) SendInput(PlayerInput.MoveDown);
    }

    public void SendInput(PlayerInput input)
    {
        currentInputs[input] = true;
    }

    private void FixedUpdate()
    {
        foreach (var listener in new List<IInputListener>(listeners))
        {
            // do actions based on the active inputs this frame

            // not moving or moving two directions at once
            if (currentInputs[PlayerInput.MoveLeft] == currentInputs[PlayerInput.MoveRight])
            {
                listener.HorzMoveTowards(0);
            }
            else
            {
                int dir = currentInputs[PlayerInput.MoveLeft] ? -1 : 1;
                listener.HorzMoveTowards(dir);
            }

            if (currentInputs[PlayerInput.MoveUp] == currentInputs[PlayerInput.MoveDown])
            {
                listener.VertMoveTowards(0);
            }
            else
            {
                int dir = currentInputs[PlayerInput.MoveDown] ? -1 : 1;
                listener.VertMoveTowards(dir);
            }

            if (currentInputs[PlayerInput.JumpPress])
            {
                listener.JumpInputDown();
            }
            if (currentInputs[PlayerInput.AbilityPress])
            {
                listener.AbilityInputDown();
            }
            if (currentInputs[PlayerInput.AbilityRelease])
            {
                listener.AbilityInputUp();
            }
        }
        foreach (var key in new List<PlayerInput>(currentInputs.Keys))
        {
            // reset all inputs to false until next frame
            currentInputs[key] = false;
        }

    }

}