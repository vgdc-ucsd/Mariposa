using System;
using UnityEngine;
using UnityEngine.Jobs;

public class RespawnPoint : MonoBehaviour, IInteractable
{
    // creates an action handler that will eventually pass this object to the player when Interact() is called
    public static event Action<RespawnPoint> OnRespawnPointInteract;

    // can be changed in case it differs from zero
    private const float POS_Z = 0f;
    // Toggles debug messages
    private bool respawnDebug = false;

    // Where to respawn the player (this is separate from the transform property as the player may need to spawn differently from the respawn point sprite)
    public Vector3 RespawnPosition = new Vector3(0f, 0f, POS_Z);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        respawnDebug = Settings.Instance.Debug.GetRespawnDebug();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // insures no memory leaks occur when scene unloads or objects are disabled / destroyed
    private void OnDisable()
    {
        OnRespawnPointInteract = null;
        if (respawnDebug) Debug.Log($"{this.gameObject.name} was cleaned up");
    }

    public Vector3 GetRespawnPosition()
    {
        return RespawnPosition;
    }

    public void SetRespawnPosition(float pos_x, float pos_y)
    {
        RespawnPosition.Set(pos_x, pos_y, POS_Z);
    }

    // Sets the player's current respawn point to the RespawnPoint object
    [ContextMenu("Set Respawn Here")]
    public void Interact()
    {
        if (respawnDebug) Debug.Log($"{gameObject.name} was interacted");

        // check to see if any object is currently listening to the action
        if (OnRespawnPointInteract != null)
        {
            // trigger action which will affects all listening objects (hopefully the player object)
            OnRespawnPointInteract?.Invoke(this);
        }
        else
        {
            if (respawnDebug) Debug.Log($"{gameObject.name} had no one to listen to (Player was not subscribed to Action)");
        }
    }
}