using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.Jobs;

public class RespawnPoint : MonoBehaviour
{
    // creates an action handler that will eventually pass this object to the player when Interact() is called
    public static event Action<RespawnPoint> OnRespawnPointInteract;

    // Toggles debug messages
    private bool respawnDebug = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        respawnDebug = Settings.Instance.Debug.GetRespawnDebug();
    }

    // insures no memory leaks occur when scene unloads or objects are disabled / destroyed
    private void OnDisable()
    {
        OnRespawnPointInteract = null;
        if (respawnDebug) Debug.Log($"{this.gameObject.name} was cleaned up");
    }

    public Vector3 GetRespawnPosition()
    {
        return transform.position;
    }

    // Sets the player's current respawn point to the RespawnPoint object
    [ContextMenu("Set Respawn Here")]
    public void SetRespawn()
    {
        if (respawnDebug) Debug.Log($"{gameObject.name} was interacted");

        // check to see if any object is currently listening to the action
        if (OnRespawnPointInteract != null)
        {
            // trigger action which will affects all listening objects (hopefully the player object)
            OnRespawnPointInteract?.Invoke(this);

            switch (Player.ActivePlayer.Data.characterID)
            {
                case CharID.Mariposa:
                    RuntimeManager.PlayOneShot("event:/sfx/world/spawnpoint_activate/mariposa");
                    break;
                case CharID.Unnamed:
                    RuntimeManager.PlayOneShot("event:/sfx/world/spawnpoint_activate/unnamed");
                    break;
            }
        }
        else
        {
            if (respawnDebug) Debug.Log($"{gameObject.name} had no one to listen to (Player was not subscribed to Action)");
        }
    }

}