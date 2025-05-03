using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineCamera cinemachineCamera;
    private CinemachineConfiner2D confiner;

    public static CameraController ActiveCamera;

    private void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
        confiner = GetComponent<CinemachineConfiner2D>();

        if (ActiveCamera != null && ActiveCamera != this)
        {
            Destroy(this);
        }
        else
        {
            ActiveCamera = this;
        }
    }

    /// <summary>
    /// Makes the camera start following the target transform
    /// </summary>
    /// <param name="target"></param>
    public void StartFollowing(Transform target)
    {
        if (target == null) return;
        cinemachineCamera.Target.TrackingTarget = target;
    }

    /// <summary>
    /// Sets the bounds for the camera
    /// </summary>
    /// <param name="newBounds">A polygon collider defining the bounds</param>
    public void SetBounds(Collider2D newBounds)
    {
        if (newBounds == null) return;
        confiner.InvalidateBoundingShapeCache();
        confiner.BoundingShape2D = newBounds;
        cinemachineCamera.PreviousStateIsValid = false;
    }
}
