using UnityEngine;

public class Crane : MonoBehaviour
{
    [SerializeField] protected LineRenderer lineRenderer;
    [SerializeField] protected CraneLoad platform;

    protected void Awake()
    {
        lineRenderer.SetPosition(0, transform.position);
    }

    protected void Update()
    {
        lineRenderer.SetPosition(1, platform.attachPoint.position);
    }

    public virtual void TriggerCrane()
    {
        platform.SendPlatform(1);
    }

    public virtual void ReturnCrane()
    {
        platform.SendPlatform(-1);
    }

}