using UnityEngine;

public class Crane : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] CraneLoad platform;

    private void Awake()
    {
        lineRenderer.SetPosition(0, transform.position);
    }

    private void Update()
    {
        lineRenderer.SetPosition(1, platform.attachPoint.position);
    }

    public void TriggerCrane()
    {
        platform.SendPlatform();
    }

}