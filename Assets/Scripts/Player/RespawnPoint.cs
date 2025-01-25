using UnityEngine;
using UnityEngine.Jobs;

public class RespawnPoint : MonoBehaviour, IInteractable
{
    private const float POS_Z = 0f;
    // Where to respawn the player (this is separate from the transform property as the player may need to spawn differently from the respawn point sprite)
    public Vector3 RespawnPosition = new Vector3(0f, 0f, POS_Z);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
    public void Interact()
    {
        // TODO: needs to set Player's CurrentRespawnPoint variable to RespawnPoint's object (maybe via event?)
    }
}