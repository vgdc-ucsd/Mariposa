using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseScript : Singleton<PauseScript>
{

    public GameObject PauseMenu;
    public bool IsPaused = false;

    private InputAction pauseInputAction;

	override public void Awake()
	{
        base.Awake();
        pauseInputAction = InputSystem.actions.FindActionMap("Player").FindAction("Escape");
	}

	void Start()
    {
        Canvas pauseCanvas = PauseMenu.GetComponent<Canvas>();
        if (pauseCanvas.worldCamera == null) pauseCanvas.worldCamera = Camera.main;
        PauseGame(false);
    }

    void Update()
    {
        if (pauseInputAction.WasPressedThisFrame())
        {
            if (IsPaused) PauseGame(false);
            else PauseGame(true);
        }
    }

    public void PauseGame(bool nextPauseState)
    {
        if (nextPauseState)
        {
            IsPaused = true;
            PauseMenu.SetActive(true);
            Time.timeScale = 0.0f;
        }
        else
        {
            IsPaused = false;
            PauseMenu.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1.0f;
        PauseGame(false);
        if (LevelManager.Instance == null)
        {
            Debug.LogWarning("Pause manager attempting to restart level but LevelManager not found!");
            return;
        }
        LevelManager.Instance.InitSublevel();
    }

    public void QuitLevel()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
}
