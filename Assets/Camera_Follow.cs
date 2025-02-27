using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Camera_Follow : MonoBehaviour
{
    [Tooltip("The default offset from the target")]
    [SerializeField] private Vector2 offset = Vector2.zero;

    [Tooltip("The time it takes for the camera to reach the target")]
    [SerializeField] private float followTime = 0.20f;

    [Tooltip("The minimum distance from the target at which the camera will warp to the target")]
    [SerializeField] private float minWarpDistance = 100f;

    private Vector2 velocity = Vector2.zero; //velocity

    [Tooltip("The transform that the camera will try to follow")]
    public Transform target;
    private Vector2 targetPosition;

    [Tooltip("Defines the region that the camera's viewport will stay inside")]
    [SerializeField] private RectTransform[] cameraBounds;
    private List<Rect> boundingRects = new(); // The region that the camera's center will stay inside
    private Rect cameraRect; // The camera's viewport rect in world coordinates

    private Camera cam;

    private void Awake()
    {
        target = Player.ActivePlayer.transform;
        targetPosition = transform.position;

        cam = GetComponent<Camera>();
        Vector2 camRectMin = cam.ViewportToWorldPoint(cam.rect.min);
        Vector2 camRectMax = cam.ViewportToWorldPoint(cam.rect.max);
        cameraRect = Rect.MinMaxRect(camRectMin.x, camRectMin.y, camRectMax.x, camRectMax.y);

        foreach (RectTransform bounds in cameraBounds)
        {
            boundingRects.Add(GetBoundingRect(bounds));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        targetPosition = (Vector2)target.position + offset;
        Debug.DrawLine(targetPosition, targetPosition + GetMinSeparation(targetPosition, boundingRects));
        targetPosition += GetMinSeparation(targetPosition, boundingRects);

        // If the camera is close enough to the target, smoothly approach it. Otherwise, warp to it.
        Vector3 newPosition = (targetPosition - (Vector2)transform.position).sqrMagnitude < minWarpDistance * minWarpDistance // Comparing squares is faster 
            ? Vector2.SmoothDamp(transform.position, targetPosition, ref velocity, followTime)
            : targetPosition;
        transform.position = newPosition + transform.position.z * Vector3.forward;
    }

    private Vector2 GetMinSeparation(Vector2 point, List<Rect> rects)
    {
        Vector2 minSeparation = Vector2.zero;
        float minSeparationDistanceSquared = float.MaxValue;
        foreach (Rect rect in rects)
        {
            Vector2 separation = Helper.MinPointRectSeparation(point, rect);
            if (separation.sqrMagnitude < minSeparationDistanceSquared)
            {
                minSeparationDistanceSquared = separation.sqrMagnitude;
                minSeparation = separation;
            }
        }

        return minSeparation;
    }

    public void SwapBounds(RectTransform[] newBounds)
    {
        boundingRects.Clear();
        foreach (RectTransform bounds in newBounds)
        {
            boundingRects.Add(GetBoundingRect(bounds));
        }
        targetPosition = transform.position + (Vector3)GetMinSeparation(transform.position, boundingRects);
    }

    /// <summary>
    /// Gets a rectangle that bounds the camera's center point
    /// </summary>
    /// <param name="rectTransform">The rect transform defining the bounds</param>
    /// <returns>A bounding rect</returns>
    public Rect GetBoundingRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Rect worldRect = new(corners[0], corners[2] - corners[0]); // these are the min and max corners
        Helper.DebugDrawRect(worldRect, Color.yellow, Mathf.Infinity);

        if (worldRect.size.sqrMagnitude < cameraRect.size.sqrMagnitude) return Rect.zero;

        // Shrink the bounding rect by the camera's rect (Minkowski difference)
        return Rect.MinMaxRect(worldRect.xMin + cameraRect.width * 0.5f, worldRect.yMin + cameraRect.height * 0.5f,
                                worldRect.xMax - cameraRect.width * 0.5f, worldRect.yMax - cameraRect.width * 0.5f);
    }
}
