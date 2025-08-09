using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float defaultOrthographicSize = 7.0f;
    private CinemachineCamera cinemachineCamera;
    private CinemachineConfiner2D confiner;

    public static CameraController ActiveCamera;

    private CinemachineTargetGroup targetGroup;
    // private bool isPaused = false;

    private bool InCameraEvent;
    private float CameraEventCtr;
    private float CameraEventDuration;

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

        // FollowPlayer();
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

    /// <summary>
    /// Pauses the camera movement
    /// </summary>
    public void PauseCamera()
    {
        // isPaused = true;
        if (cinemachineCamera != null)
            cinemachineCamera.Target.TrackingTarget = null;
    }

    /// <summary>
    /// Resumes the camera movement
    /// </summary>
    public void ResumeCamera()
    {
        // isPaused = false;
        if (Player.ActivePlayer != null)
            StartFollowing(Player.ActivePlayer.transform);
    }

    public void FollowPlayer()
    {
        if (Player.ActivePlayer != null)
            StartFollowing(Player.ActivePlayer.transform);
    }

    /// <summary>
    ///  reset camera to follow the player
    /// </summary>
    public void ResetToDefault()
    {
        FollowPlayer();
        targetGroup.Targets.Clear();
        CinemachineTargetGroup.Target playerCTG = new()
        {
            Object = Player.ActivePlayer.transform,
            Radius = 1.0f,
            Weight = 1.0f
        };
        targetGroup.Targets.Add(playerCTG);

        CameraEventCtr = 0.0f;
        InCameraEvent = false;
        cinemachineCamera.Lens.OrthographicSize = defaultOrthographicSize;
    }

    private void CreateSimpleCameraEvent(Transform position, float depth, float time)
    {
        InCameraEvent = true;
        CameraEventCtr = 0.0f;
        CameraEventDuration = time;
        cinemachineCamera.Lens.OrthographicSize = depth;
        StartFollowing(position);
    }

    /// <summary>
    /// function to try to move the camera to a specific spot, at a specific depth, during a specific time
    /// cannot do this if camera event is already happening
    /// </summary>
    /// <param name="position">the transform you want the camera to lock on to</param>
    /// <param name="depth">the depth (how far away the camera is from z=0)</param>
    /// <param name="time">how long you want the camera to do this in seconds</param>
    public void TrySimpleCameraEvent(Transform position, float depth, float time)
    {
        if (InCameraEvent) return;

        CreateSimpleCameraEvent(position, depth, time);
    }

    /// <summary>
    /// function to force the camera to a specific spot, at a specific depth, during a specific time
    /// </summary>
    /// <param name="position">the transform you want the camera to lock on to</param>
    /// <param name="depth">the depth (how far away the camera is from z=0)</param>
    /// <param name="time">how long you want the camera to do this in seconds</param>
    public void ForceSimpleCameraEvent(Transform position, float depth, float time)
    {
        CreateSimpleCameraEvent(position, depth, time);
    }

    // doesnt work right now
    private void MoveCameraToCoverObjects(Transform[] positionsToCover, float time)
    {
        // cinemachineCamera.Target.TrackingTarget = null;
        targetGroup.Targets.Clear();
        InCameraEvent = true;
        CameraEventCtr = 0.0f;
        CameraEventDuration = time;

        foreach (Transform t in positionsToCover)
        {
            CinemachineTargetGroup.Target objectCTG = new()
            {
                Object = t,
                Radius = 1.0f,
                Weight = 1.0f
            };
            targetGroup.Targets.Add(objectCTG);
        }
    }

    /// <summary>
    /// force the camera to cover a specific set of objects for a set amount of time (doesnt work)
    /// </summary>
    /// <param name="positionsToCover">the transforms you want the camera to cover</param> 
    /// <param name="time">how long you want the camera to do this in seconds</param>
    public void ForceMoveCameraToCoverObjects(Transform[] positionsToCover, float time)
    {
        MoveCameraToCoverObjects(positionsToCover, time);
    }

    /// <summary>
    /// try to get the camera to cover a specific set of objects for a set amount of time (doesnt work)
    /// cannot do this if camera event is already happening
    /// </summary>
    /// <param name="positionsToCover">the transforms you want the camera to cover</param> 
    /// <param name="time">how long you want the camera to do this in seconds</param>
    public void TryMoveCameraToCoverObjects(Transform[] positionsToCover, float time)
    {
        if (InCameraEvent) return;

        MoveCameraToCoverObjects(positionsToCover, time);
    }

    void Update()
    {
        if (!InCameraEvent) return;

        float dt = Time.deltaTime;
        CameraEventCtr += dt;

        if (CameraEventCtr > CameraEventDuration)
        {
            ResetToDefault();
        } 
    }
}
