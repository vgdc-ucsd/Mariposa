using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
	public RespawnPoint CurrentRespawnPoint = null;

	public static Player ActivePlayer;
	private const bool PLAYER_DEBUG = false;

	private void Awake()
	{
		if (ActivePlayer == null)
		{
			ActivePlayer = this;
		}
	}


	// when initialized / reactivated, have the player listen for any respawn point interactions and update it as needed
	// NOTE: this may introduce memory leaks or issues if i didnt implement the failsafes correctly
	private void OnEnable()
	{
		RespawnPoint.OnRespawnPointInteract -= UpdateRespawn; // insures listener is empty
		RespawnPoint.OnRespawnPointInteract += UpdateRespawn;
		if (PLAYER_DEBUG) Debug.Log("Player is now listening for respawn interacts");
	}

	// insures no memory leaks occur when scene unloads or player is destroyed
	private void OnDisable()
	{
		RespawnPoint.OnRespawnPointInteract -= UpdateRespawn;
		if (PLAYER_DEBUG) Debug.Log("Player was cleaned up");
	}

	private void Update()
	{

	}

	private void FixedUpdate()
	{

	}

	// serves both as a manual updater of the player's current respawn point and as the function to be called when a respawn point is interacted
	private void UpdateRespawn(RespawnPoint receivedRespawnPoint)
	{
		CurrentRespawnPoint = receivedRespawnPoint;
		if (PLAYER_DEBUG) Debug.Log($"Player's current respawn point updated to: {CurrentRespawnPoint.gameObject.name} @ {CurrentRespawnPoint.GetRespawnPosition().ToString()}");
	}

	// when respawning, change position to current respawn position. if not found, default to origin
	[ContextMenu("Respawn")]
	public void Respawn()
	{
		if (CurrentRespawnPoint == null)
		{
			transform.position = new Vector3(0f, 0f, transform.position.z);
			if (PLAYER_DEBUG) Debug.Log($"Player respawned to: {transform.position.ToString()}");
		}
		else
		{
			transform.position = CurrentRespawnPoint.GetComponent<RespawnPoint>().GetRespawnPosition();
			if (PLAYER_DEBUG) Debug.Log($"Player respawned to: {CurrentRespawnPoint.gameObject.name} @ {CurrentRespawnPoint.GetRespawnPosition().ToString()}");
		}
	}
}
