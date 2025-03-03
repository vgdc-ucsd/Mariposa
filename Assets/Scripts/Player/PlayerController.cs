using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

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

    private InputSystem_Actions inputs;

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
        inputs = new();
    }

    private void OnEnable()
    {
        inputs.Enable();
        inputs.Player.Ability.started += ctx => SendAbilityDown(ctx);
        inputs.Player.Ability.canceled += ctx => SendAbilityUp(ctx);
        inputs.Player.Jump.performed += ctx => SendJump(ctx);
    }

    private void OnDisable()
    {
        inputs.Player.Ability.started -= ctx => SendAbilityDown(ctx);
        inputs.Player.Ability.canceled -= ctx => SendAbilityUp(ctx);
        inputs.Player.Jump.performed -= ctx => SendJump(ctx);
        inputs.Disable();
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
        if (CurrentControllable != null) CurrentControllable.SetMoveDir(Vector2.zero);
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
        // TEMPORARY AND SHOULD BE REMOVED IN ANY NON-TEST BUILD
        if (Input.GetKeyDown(KeyCode.Tab)) SwitchCharacters();
        if (Input.GetKeyDown(KeyCode.E)) SendInteract();
    }

    public void SendAbilityDown(InputAction.CallbackContext ctx)
    {
        listeners.ForEachReverse(x => x.AbilityInputDown());
    }

    public void SendAbilityUp(InputAction.CallbackContext ctx)
    {
        listeners.ForEachReverse(x => x.AbilityInputUp());
    }

    public void SendJump(InputAction.CallbackContext ctx)
    {
        listeners.ForEachReverse(x => x.JumpInputDown());
    }

    public void SendInteract()
    {
        listeners.ForEachReverse(x => x.InteractInputDown());
    }

    private void FixedUpdate()
    {
        Vector2 moveDir = inputs.Player.Move.ReadValue<Vector2>();
        foreach (var listener in new List<IInputListener>(listeners))
        {
            listener.SetMoveDir(moveDir);
        }
    }
}