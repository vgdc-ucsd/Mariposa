using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;

    [Header("Character References")]
    public GameObject unknown;    // Character for World A
    public GameObject mariposa;   // Character for World B

    [Header("World References")]
    public GameObject worldA;     // Parent GameObject for all levels in World A
    public GameObject worldB;     // Parent GameObject for all levels in World B

    private bool isWorldAActive = true;  // Tracks which world is currently active
    private bool isSwitching = false;    // Prevents simultaneous switches

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ensure only one world is active at the start
        worldA.SetActive(true);
        worldB.SetActive(false);

        unknown.SetActive(true);
        mariposa.SetActive(false);
    }

    private void Update()
    {
        // Swap worlds when the "F" key is pressed
        if (Input.GetKeyDown(KeyCode.F) && !isSwitching)
        {
            SwitchWorlds();
        }
    }

    public void SwitchWorlds()
    {
        if (isSwitching) return;  // Prevent multiple switches at once
        isSwitching = true;

        if (isWorldAActive)
        {
            // Swap to World B
            StartCoroutine(SwitchToWorld(worldB, mariposa, worldA, unknown));
        }
        else
        {
            // Swap back to World A
            StartCoroutine(SwitchToWorld(worldA, unknown, worldB, mariposa));
        }
    }

    private IEnumerator SwitchToWorld(GameObject worldToActivate, GameObject characterToActivate, GameObject worldToDeactivate, GameObject characterToDeactivate)
    {
        // Deactivate old world and character
        worldToDeactivate.SetActive(false);
        characterToDeactivate.SetActive(false);

        yield return null; // Let Unity process the changes


        // Activate the new world and character
        worldToActivate.SetActive(true);
        characterToActivate.SetActive(true);
        PlayerController.Instance.SwitchCharacters();
        // Update active world tracker
        isWorldAActive = (characterToActivate == unknown);
        isSwitching = false;

        Debug.Log($"Switched to {worldToActivate.name} and activated {characterToActivate.name}.");
    }
}
