using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CharID
{
    Mariposa, Unnamed
}
public enum PlayerInput
{
    MoveLeft, MoveRight, MoveUp, MoveDown, JumpPress, AbilityPress, AbilityRelease
}

// Handles delegating inputs to the active player, as well as managing switching between the two characters
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Player MariposaRef;
    [SerializeField]
    private Player UnnamedRef;

    private Dictionary<CharID, Player> charIDMap = new();


    public static PlayerController Instance;
    private List<IInputListener> listeners = new();

    public CharID StartingPlayer;
    public Player ControlledPlayer;
    public IControllable CurrentControllable;



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
        if (MariposaRef == null || UnnamedRef == null)
        {
            Debug.LogError("Characters are not assigned to Player Controller");
        }
        charIDMap.Add(CharID.Mariposa, MariposaRef);
        charIDMap.Add(CharID.Unnamed, UnnamedRef);

        InitializePlayers();
    }

    private void InitializePlayers()
    {
        foreach (Player player in new Player[] { MariposaRef, UnnamedRef })
        {
            player.gameObject.SetActive(false);
        }
        SwitchTo(StartingPlayer);

    }

    public void SwitchCharacters()
    {
        if (ControlledPlayer.Character.Id == CharID.Mariposa) SwitchTo(CharID.Unnamed);
        else SwitchTo(CharID.Mariposa);  
    }




    public void SwitchTo(CharID character)
    {
        if (ControlledPlayer != null) ControlledPlayer.gameObject.SetActive(false);

        // remove all input listeners that belong to a character
        foreach (IInputListener listener in new List<IInputListener>(listeners))
        {
            if (listener is IControllable || listener is IAbility)
            {
                listeners.Remove(listener);
            }
        }


        ControlledPlayer = charIDMap[character];
        ControlledPlayer.gameObject.SetActive(true);
        StartControlling(ControlledPlayer.Movement);
        Subscribe(ControlledPlayer.Ability);
        ControlledPlayer.Ability.Initialize();
        
    }

    // map inputs to this controllable and make it the camera target
    public void StartControlling(IControllable controllable)
    {
        Unsubscribe(CurrentControllable);
        Subscribe(controllable);
        CurrentControllable = controllable;
        CameraController.ActiveCamera.StartFollowing(controllable.transform);
    }

    /* INPUT HANDLING */

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

        if (Input.GetKeyDown(KeyCode.Tab)) SwitchCharacters();
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