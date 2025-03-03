using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{


	public RespawnPoint CurrentRespawnPoint = null;

	public static Player ActivePlayer => PlayerController.Instance.ControlledPlayer;


	private bool playerDebug;
	public PlayerCharacter Character;
	public PlayerMovement Movement;
	public IAbility Ability;


	// which way the character is facing
	// 1 = right, -1 = left, can never be 0
	// facing direction does not affect movement in most cases
	public int FacingDirection = 1;

	private void Awake()
	{
		Movement = GetComponent<PlayerMovement>();
		Movement.Parent = this;
		Character = GetComponent<PlayerCharacter>();
		Ability = GetComponentInChildren<IAbility>();
		if (Movement == null || Character == null || Ability == null)
		{
			Debug.LogError("Player object not fully set up with Movement, Character, and Ability classes");
			return;
		}
	}

	void Start()
	{
		playerDebug = Settings.Instance.Debug.GetPlayerDebug();
	}


	// when initialized / reactivated, have the player listen for any respawn point interactions and update it as needed
	// NOTE: this may introduce memory leaks or issues if i didnt implement the failsafes correctly
	private void OnEnable()
	{
		RespawnPoint.OnRespawnPointInteract -= UpdateRespawn; // insures listener is empty
		RespawnPoint.OnRespawnPointInteract += UpdateRespawn;
		if (playerDebug) Debug.Log("Player is now listening for respawn interacts");
	}

	// insures no memory leaks occur when scene unloads or player is destroyed
	private void OnDisable()
	{
		RespawnPoint.OnRespawnPointInteract -= UpdateRespawn;
		if (playerDebug) Debug.Log("Player was cleaned up");
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
		if (playerDebug) Debug.Log($"Player's current respawn point updated to: {CurrentRespawnPoint.gameObject.name} @ {CurrentRespawnPoint.GetRespawnPosition().ToString()}");
	}

	// when respawning, change position to current respawn position. if not found, default to origin
	[ContextMenu("Respawn")]
	public void Respawn()
	{
		if (CurrentRespawnPoint == null)
		{
			transform.position = new Vector3(0f, 0f, transform.position.z);
			if (playerDebug) Debug.Log($"Player respawned to: {transform.position.ToString()}");
		}
		else
		{
			Movement.Velocity = Vector2.zero;
			transform.position = CurrentRespawnPoint.GetComponent<RespawnPoint>().GetRespawnPosition();
			Movement.ResolveInitialCollisions();
			if (playerDebug) Debug.Log($"Player respawned to: {CurrentRespawnPoint.gameObject.name} @ {CurrentRespawnPoint.GetRespawnPosition().ToString()}");
			RuntimeManager.PlayOneShot("event:/sfx/player/respawn");
		}
	}

	public void TurnTowards(int dir)
	{
		FacingDirection = dir;
	}

	public void Die()
	{
		// TODO: there may be not that much delay between death and respawn, so remove the below line or add a delay after this line to prevent it overlapping with respawn sfx
		RuntimeManager.PlayOneShot("event:/sfx/player/death");
		Respawn();
	}

	public void ObtainCheckpoint(GameObject checkpoint)
	{
		UpdateRespawn(checkpoint.GetComponent<RespawnPoint>());
		checkpoint.GetComponent<Collider2D>().enabled = false;

		switch (Player.ActivePlayer.Character.Name)
		{
			case "Mariposa":
				RuntimeManager.PlayOneShot("event:/sfx/puzzle/spawnpoint_activate/mariposa");
				break;
			case "Unnamed":
				RuntimeManager.PlayOneShot("event:/sfx/puzzle/spawnpoint_activate/unnamed");
				break;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		switch (collision.gameObject.tag)
		{
			case "Death": Die(); break;
			case "Checkpoint": ObtainCheckpoint(collision.gameObject); break;
		}
	}
}
