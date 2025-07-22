using UnityEngine;

public class GrappleTarget : MonoBehaviour
{
    [SerializeField] private GameObject targetedGO;
    [SerializeField] private GameObject untargetedGO;
    [SerializeField] bool oneUse;
    [SerializeField] private float respawnTime = 3.0f;
    private float respawn_t = 0;
    private bool isTargeted;

    public bool isAvailable = true;

    private void Awake()
    {
        ResetGrappleTarget();
    }

    private void OnEnable()
    {
        Player.OnDeath += ResetGrappleTarget;
    }

    private void OnDisable()
    {
        Player.OnDeath -= ResetGrappleTarget;
    }

    private void Update()
    {
        respawn_t -= Time.deltaTime;

        if (respawn_t <= 0 && !isTargeted)
        {
            untargetedGO.SetActive(true);
            isAvailable = true;
        }
    }

    public void ToggleHighlight(bool toggle)
    {
        isTargeted = toggle;
        targetedGO.SetActive(toggle);
        untargetedGO.SetActive(!toggle);
    }

    public virtual void ReleaseGrapple()
    {
        if (oneUse)
        {
            targetedGO.SetActive(false);
            untargetedGO.SetActive(false);
            isAvailable = false;
            respawn_t = respawnTime;
        }
    }

    public void ResetGrappleTarget()
    {
        isAvailable = true;
        respawn_t = 0;
        isTargeted = false;
    }
}
