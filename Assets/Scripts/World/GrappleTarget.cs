using UnityEngine;

public class GrappleTarget : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] bool oneUse;
    [SerializeField] private float respawnTime = 3.0f;
    private float respawn_t = 0;

    public bool isAvailable = true;

    private void Update()
    {
        respawn_t -= Time.deltaTime;

        if (respawn_t <= 0)
        {
            sr.enabled = true;
            isAvailable = true;
        }
    }

    public void ToggleHighlight(bool toggle)
    {
        sr.color = toggle ? Color.green : Color.yellow;
    }

    public virtual void ReleaseGrapple()
    {
        if (oneUse)
        {
            sr.enabled = false;
            isAvailable = false;
            respawn_t = respawnTime;
        }
    }
}
