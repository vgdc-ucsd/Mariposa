using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// These must be in the same order as the Unity build settings
public enum GameScene
{
    MAIN_MENU,
    TUTORIAL,
    DOWNTOWN,
    PIER,
    ROBOT,
    HOMETOWN,
    CREDITS
};

public enum GameState
{
    GAME,
    PAUSE,
    INVENTORY
}

public class GameManager : Singleton<GameManager>, IDataPersistence
{
    public GameScene CurrentScene { get; private set; }
    public GameScene SavedScene { get; private set; }
    public InteractionTrigger DefaultInteractionTrigger;
    private InputSystem_Actions actions;

    public StateMachine<GameState> GameStateMachine { get; private set; }

    public override void Awake()
    {
        actions = new InputSystem_Actions();
        CurrentScene = (GameScene)SceneManager.GetActiveScene().buildIndex;
        base.Awake();
    }

    void Start()
    {
        GameStateMachine = new StateMachine<GameState>(GameState.GAME);

        // Game
        GameStateMachine.AddTransition(GameState.GAME, GameState.PAUSE);
        GameStateMachine.AddTransition(GameState.GAME, GameState.INVENTORY);
        GameStateMachine.AddEnterAction(GameState.GAME, EnterGame);
        GameStateMachine.AddExitAction(GameState.GAME, ExitGame);

        // Pause
        GameStateMachine.AddTransition(GameState.PAUSE, GameState.GAME);

        // Inventory
        GameStateMachine.AddTransition(GameState.INVENTORY, GameState.GAME);
        GameStateMachine.AddTransition(GameState.INVENTORY, GameState.PAUSE);
    }

    public void LoadScene(GameScene scene)
    {
        OnChangeScene();
        CurrentScene = scene;
        SceneManager.LoadSceneAsync((int)scene);
    }

    public void OnChangeScene()
    {
        UnregisterStartAction(GameState.INVENTORY);
        UnregisterExitAction(GameState.INVENTORY);
    }

    public void RegisterStartAction(GameState state, Action action)
    {
        GameStateMachine.AddEnterAction(state, action);
    }

    public void UnregisterStartAction(GameState state)
    {
        GameStateMachine.RemoveEnterAction(state);
    }

    public void RegisterExitAction(GameState state, Action action)
    {
        GameStateMachine.AddExitAction(state, action);
    }

    public void UnregisterExitAction(GameState state)
    {
        GameStateMachine.RemoveExitAction(state);
    }

    public void HandlePause()
    {
        if (CurrentScene == GameScene.MAIN_MENU || CurrentScene == GameScene.CREDITS) PauseMenuScript.Instance.ResumeGame();
        else if (GameStateMachine.GetState() == GameState.PAUSE) GameStateMachine.Transition(GameState.GAME);
        else GameStateMachine.Transition(GameState.PAUSE);
    }

    public void HandleInventory()
    {
        if (GameStateMachine.GetState() == GameState.GAME) GameStateMachine.Transition(GameState.INVENTORY);
        else if (GameStateMachine.GetState() == GameState.INVENTORY) GameStateMachine.Transition(GameState.GAME);
    }

    private void EnterGame()
    {
        Time.timeScale = 1.0f;
        actions.Player.Enable();
    }

    private void ExitGame()
    {
        actions.Player.Disable();
    }

    void OnEnable()
    {
        actions.Player.Enable();
        actions.Control.Enable();
        actions.Control.Escape.started += ctx => HandlePause();
        actions.Control.Inventory.started += ctx => HandleInventory();
    }

    void OnDisable()
    {
        actions.Control.Escape.started -= ctx => HandlePause();
        actions.Control.Inventory.started -= ctx => HandleInventory();
        actions.Control.Disable();
        actions.Player.Disable();
    }

    public void SaveData(ref GameData data)
    {
        data.SavedScene = SavedScene;
    }

    public void LoadData(GameData data)
    {
        SavedScene = data.SavedScene;
    }
}
