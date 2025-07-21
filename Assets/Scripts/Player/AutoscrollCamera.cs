using System.Collections;

using Unity.Cinemachine;
using UnityEngine;

public class AutoscrollCamera : CameraController
{
    [Header("References")]
    private CinemachineSplineDolly dolly;
    private CinemachineCamera virtualCam;
    private Camera cam;

    [Header("Autoscrolling")]
    [SerializeField] private float movementStartDelay = 2.0f;
    [SerializeField] private float baseCameraMoveSpeed = 5f;
    [SerializeField] private float speedMultiplier = 1f;

    [Header("Player Tracking")]
    [SerializeField] private Vector2 screenDeadZone = new(0.3f, 0.2f); // Normalized screen space
    [SerializeField] private Vector2 screenBounds = new(0.8f, 0.6f); // Max allowed player position
    [SerializeField] private float trackingDamping = 3f;
    [SerializeField] private float maxTrackingOffset = 5f;

    [Header("Dynamic Speed")]
    [SerializeField] private float speedBoostMultiplier = 1.5f;
    [SerializeField] private float speedReduceMultiplier = 0.7f;
    [SerializeField] private float speedAdjustmentRate = 2f;
    
    [Header("Thresholds")]
    [SerializeField] private float catchUpThreshold = 0.9f; // Screen position where catch-up starts
    [SerializeField] private float slowDownThreshold = 0.3f; // Screen position where slow-down starts
    [SerializeField] private float killThreshold = 1.1f; // Screen position where kill starts

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private Vector2 prevCamPos;
    private Vector3 currentTrackingOffset;
    private Vector3 targetTrackingOffset;
    private float targetSpeedMultiplier = 1f;
    private bool isMoving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        virtualCam = GetComponent<CinemachineCamera>();
        dolly = GetComponent<CinemachineSplineDolly>();
        cam = Camera.main;

        ResetCamera();
    }

    public void ResetCamera()
    {
        isMoving = false;
        dolly.CameraPosition = 0;
        prevCamPos = cam.transform.position;

        StartCoroutine(StartMoving());
    }

    private IEnumerator StartMoving()
    {
        isMoving = false;

        yield return new WaitForSeconds(movementStartDelay);

        isMoving = true;
    }

    private void OnEnable()
    {
        Player.OnDeath += ResetCamera;
    }

    private void OnDisable()
    {
        Player.OnDeath -= ResetCamera;
    }

    public void Update()
    {
    }

    void FixedUpdate()
    {
        float fdt = Time.fixedDeltaTime;

        if (!isMoving) return;

        Vector3 playerScreenPos = cam.WorldToViewportPoint(Player.ActivePlayer.transform.position);
        Vector2 centerOffset = new(playerScreenPos.x - 0.5f, playerScreenPos.y - 0.5f);
        Vector2 trackingInput = ApplyDeadZone(centerOffset, screenDeadZone);
        trackingInput = new(
            Mathf.Clamp(trackingInput.x, -screenBounds.x, screenBounds.x),
            Mathf.Clamp(trackingInput.y, -screenBounds.y, screenBounds.y)
        );

        targetTrackingOffset = new(
            trackingInput.x * maxTrackingOffset,
            trackingInput.y * maxTrackingOffset,
            0f
        );

        currentTrackingOffset = Vector3.Lerp(currentTrackingOffset, targetTrackingOffset, trackingDamping * fdt);
        dolly.SplineOffset = currentTrackingOffset;

        // Determine target speed based on player position
        targetSpeedMultiplier = 1f;
        Vector2 camMoveDir = ((Vector2)cam.transform.position - prevCamPos).normalized;
        Vector2 proj = Helper.Vec2Proj(centerOffset, camMoveDir);
        float projFrac = Vector2.Dot(proj, camMoveDir.normalized * proj.magnitude);
        Debug.Log("proj: " + proj + "projFrac: " + projFrac);
        if (projFrac > catchUpThreshold)
            targetSpeedMultiplier = speedBoostMultiplier;
        else if (projFrac < killThreshold) 
            Player.ActivePlayer.Die();
        else if (projFrac < slowDownThreshold)
            targetSpeedMultiplier = speedReduceMultiplier;
        
        speedMultiplier = Mathf.Lerp(speedMultiplier, targetSpeedMultiplier, speedAdjustmentRate * fdt);

        dolly.CameraPosition += baseCameraMoveSpeed * speedMultiplier * fdt;
        prevCamPos = cam.transform.position;
    }
    
    Vector2 ApplyDeadZone(Vector2 input, Vector2 deadZone)
    {
        Vector2 result = Vector2.zero;
        
        // Apply dead zone to X axis
        if (Mathf.Abs(input.x) > deadZone.x)
        {
            result.x = Mathf.Sign(input.x) * ((Mathf.Abs(input.x) - deadZone.x) / (0.5f - deadZone.x));
        }
        
        // Apply dead zone to Y axis
        if (Mathf.Abs(input.y) > deadZone.y)
        {
            result.y = Mathf.Sign(input.y) * ((Mathf.Abs(input.y) - deadZone.y) / (0.5f - deadZone.y));
        }
        
        return result;
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        
        // Convert screen bounds to world positions for visualization
        Vector3 screenCenter = cam.ScreenToWorldPoint(new Vector3(
            Screen.width * 0.5f, Screen.height * 0.5f, 10f));
        
        Vector3 deadZoneSize = cam.ScreenToWorldPoint(new Vector3(
            Screen.width * screenDeadZone.x, Screen.height * screenDeadZone.y, 10f)) - 
            cam.ScreenToWorldPoint(Vector3.zero);
        
        Vector3 boundsSize = cam.ScreenToWorldPoint(new Vector3(
            Screen.width * screenBounds.x, Screen.height * screenBounds.y, 10f)) - 
            cam.ScreenToWorldPoint(Vector3.zero);
        
        // Draw dead zone
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(screenCenter, deadZoneSize * 2f);
        
        // Draw tracking bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(screenCenter, boundsSize * 2f);
        
        // Draw player screen position indicator
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Player.ActivePlayer.transform.position, 0.5f);
        
        // Draw current tracking offset
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(virtualCam.transform.position, 
            virtualCam.transform.position + currentTrackingOffset);
    }
}
