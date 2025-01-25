using UnityEngine;

public class Player : MonoBehaviour
{
	public RespawnPoint CurrentRespawnPoint = null;
	// TODO: RespawnPoint's Interact() function needs to be able to change the CurrentRespawnPoint variable

	private void Awake()
	{

	}

	private void Start()
	{

	}

	private void Update()
	{

	}

	private void FixedUpdate()
	{

	}

	// when respawning, change position to current respawn position. if not found, default to origin
	public void Respawn()
	{
		if (CurrentRespawnPoint == null)
		{
			transform.position = new Vector3(0f, 0f, 0f);
		}
		else
		{
			transform.position = CurrentRespawnPoint.GetComponent<RespawnPoint>().GetRespawnPosition();
		}
	}
}
