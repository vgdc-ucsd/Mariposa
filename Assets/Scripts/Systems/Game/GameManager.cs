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
    HOMETOWN
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
    public InteractionTrigger DefaultInteractionTrigger;
    private InputSystem_Actions actions;

    private StateMachine<GameState> gameState;

    public override void Awake()
    {
        actions = new InputSystem_Actions();
        CurrentScene = (GameScene)SceneManager.GetActiveScene().buildIndex;
        base.Awake();
    }

    void Start()
    {
        gameState = new StateMachine<GameState>(GameState.GAME);

        // Game
        gameState.AddTransition(GameState.GAME, GameState.PAUSE);
        gameState.AddTransition(GameState.GAME, GameState.INVENTORY);
        gameState.AddEnterAction(GameState.GAME, EnterGame);
        gameState.AddExitAction(GameState.GAME, ExitGame);

        // Pause
        gameState.AddTransition(GameState.PAUSE, GameState.GAME);

        // Inventory
        gameState.AddTransition(GameState.INVENTORY, GameState.GAME);
        gameState.AddTransition(GameState.INVENTORY, GameState.PAUSE);
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
        UnregisterStartAction(GameState.PAUSE);
        UnregisterExitAction(GameState.PAUSE);
    }

    public void RegisterStartAction(GameState state, Action action)
    {
        gameState.AddEnterAction(state, action);
    }

    public void UnregisterStartAction(GameState state)
    {
        gameState.RemoveEnterAction(state);
    }

    public void RegisterExitAction(GameState state, Action action)
    {
        gameState.AddExitAction(state, action);
    }

    public void UnregisterExitAction(GameState state)
    {
        gameState.RemoveExitAction(state);
    }

    public void HandlePause()
    {
        if (CurrentScene == GameScene.MAIN_MENU) MainMenuSettings.Instance.CloseAllMenus();
        else if (gameState.GetState() == GameState.PAUSE) gameState.Transition(GameState.GAME);
        else gameState.Transition(GameState.PAUSE);
    }

    public void HandleInventory()
    {
        if (gameState.GetState() == GameState.GAME) gameState.Transition(GameState.INVENTORY);
        else if (gameState.GetState() == GameState.INVENTORY) gameState.Transition(GameState.GAME);
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
        data.SavedScene = CurrentScene;
    }

    public void LoadData(GameData data)
    {
        CurrentScene = data.SavedScene;
    }
}
