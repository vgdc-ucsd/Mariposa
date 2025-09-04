using System;
using System.Runtime.Serialization;
using System.Collections;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Unity.Mathematics;

public class Player : MonoBehaviour
{


	public RespawnPoint CurrentRespawnPoint = null;

	public static Player ActivePlayer => PlayerController.Instance.ControlledPlayer;

    public static event Action OnDeath;

	private bool playerDebug;
	public PlayerCharacter Character;
	public PlayerMovement Movement;
	public IAbility Ability;
	public PlayerData Data;

	private readonly string[] barrierLayer = new string[] { "Barrier" };

	// which way the character is facing
	// 1 = right, -1 = left, can never be 0
	// facing direction does not affect movement in most cases
	public int FacingDirection = 1;

	[SerializeField, Min(0f)] private float characterVocalizationAttemptInterval = 15.0f;
	[SerializeField, Range(0f, 1f)] private float characterVocalizationChance = 0.1f;
	private float characterVocalizationTime;

	private void Awake()
	{
		Movement = GetComponent<PlayerMovement>();
		Ability = GetComponentInChildren<IAbility>();
		if (Movement == null || Data == null || Ability == null)
		{
			Debug.LogError("Player object not fully set up with Movement, Character, and Ability classes");
			return;
		}

		characterVocalizationTime = 0.0f;
	}

	void Start()
	{
		playerDebug = GameManager.Instance.Debug.PlayerDebugEnabled;
	}


	// when initialized / reactivated, have the player listen for any respawn point interactions and update it as needed
	// NOTE: this may introduce memory leaks or issues if i didnt implement the failsafes correctly
	private void OnEnable()
	{
		RespawnPoint.OnRespawnPointInteract -= UpdateRespawn; // insures listener is empty
		RespawnPoint.OnRespawnPointInteract += UpdateRespawn;
        OnDeath += Respawn;
		if (playerDebug) Debug.Log("Player is now listening for respawn interacts");
	}

	// insures no memory leaks occur when scene unloads or player is destroyed
	private void OnDisable()
	{
		RespawnPoint.OnRespawnPointInteract -= UpdateRespawn;
        OnDeath -= Respawn;
		if (playerDebug) Debug.Log("Player was cleaned up");
	}

	private void Update()
	{
		characterVocalizationTime += Time.deltaTime;

		if (characterVocalizationTime >= characterVocalizationAttemptInterval)
		{
			float outcome = UnityEngine.Random.value;
			if (outcome <= characterVocalizationChance)
			{
				EventReference vocalization = Data.characterID == CharID.Mariposa
					? AudioEvents.SFX.mariposa_hum_motif
					: AudioEvents.SFX.unnamed_whistle_unnamed_motif;
				RuntimeManager.PlayOneShot(vocalization);
			}
			characterVocalizationTime = 0.0f;
		}
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
			Movement.ResolveInitialCollisions();
			if (playerDebug) Debug.Log($"Player respawned to: {transform.position.ToString()}");
		}
		else
		{
			Movement.Velocity = Vector2.zero;
			SpawnAt(CurrentRespawnPoint.GetComponent<RespawnPoint>().GetRespawnPosition());			
			if (playerDebug) Debug.Log($"Player respawned to: {CurrentRespawnPoint.gameObject.name} @ {CurrentRespawnPoint.GetRespawnPosition().ToString()}");
		}
		LevelManager.Instance.RestartFromCheckpoint();
    }

	public void SpawnAt(Vector3 spawn)
	{
		Vector2 spawn2D = spawn;
		RaycastHit2D hit = Physics2D.Raycast(spawn2D, Vector2.down, 1000f, LayerMask.GetMask(barrierLayer));
		if (hit)
		{
			// Slight vertical offset to favor pushing up when resolving initial collisions
			transform.position = hit.point + Vector2.up * 1.5f;
		}
		else
		{
			Debug.LogError($"Unable to respawn at the spawnpoint at {spawn}");
		}
		Movement.ResolveInitialCollisions();
	}

	public void TurnTowards(int dir)
	{
		FacingDirection = dir;
	}

	public IEnumerator Die()
	{
        // TODO: there may be not that much delay between death and respawn, so remove the below line or add a delay after this line to prevent it overlapping with respawn sfx
        OnDeath.Invoke();
		if (Data.characterID == CharID.Unnamed) RuntimeManager.PlayOneShot(AudioEvents.SFX.unnamed_pain);
		else Debug.LogError($"Died as {Data.characterID}");
		SetPlayerActive(false);
		CameraController.ActiveCamera?.PauseCamera();
		yield return FadeController.Instance.FadeOut();
		Respawn();
		SetPlayerActive(true);
		CameraController.ActiveCamera?.ResumeCamera();
		FadeController.Instance.FadeIn();
	}
	
	private void SetPlayerActive(bool active)
	{
		foreach (var renderer in GetComponentsInChildren<Renderer>())
			renderer.enabled = active;
		foreach (var col in GetComponentsInChildren<Collider2D>())
			col.enabled = active;
		Movement.Velocity = Vector2.zero;
	}


	private IEnumerator FadeEffectAfterRespawn()
	{
		yield return FadeController.Instance.FadeOut();
		yield return new WaitForSeconds(0.1f);
		FadeController.Instance.FadeIn();
	}
	
	public void ObtainCheckpoint(GameObject checkpoint)
	{
		UpdateRespawn(checkpoint.GetComponent<RespawnPoint>());
		checkpoint.GetComponent<Collider2D>().enabled = false;

		switch (Player.ActivePlayer.Data.characterID)
		{
			case CharID.Mariposa:
				RuntimeManager.PlayOneShot(AudioEvents.SFX.spawnpoint_activate_mariposa);
				break;
			case CharID.Unnamed:
				RuntimeManager.PlayOneShot(AudioEvents.SFX.spawnpoint_activate_unnamed);
				break;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		switch (collision.gameObject.tag)
		{
			case "Death": StartCoroutine(Die()); break;
			case "Checkpoint": ObtainCheckpoint(collision.gameObject); break;
		}
	}
}
