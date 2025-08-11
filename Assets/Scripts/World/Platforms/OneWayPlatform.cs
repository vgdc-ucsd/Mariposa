using Unity.VisualScripting;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private Collider2D col;

    [HideInInspector] public Vector2 collisionNormal { get; private set; } // This will always be the surface normal closest to the up vector

    [Tooltip("The maximum angle in degrees between this collider's collision surface normal" +
        "and the opposite of the movement direction to cause a collision")] 
    [Range(0f, 360f)] public float surfaceArc = 90f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider2D>();
        SetCollisionNormal();
        gameObject.tag = "OneWayPlatform";
    }

    // This function only works if the collider is convex.
    private void SetCollisionNormal()
    {
        if (col is BoxCollider2D)
        {
            float normalRotationAngle = Mathf.Repeat(transform.rotation.eulerAngles.z, 90.0f);
            collisionNormal = Quaternion.Euler(0.0f, 0.0f, normalRotationAngle) * Vector2.up;
        }
        else if (col is PolygonCollider2D polyCol)
        {
            if (polyCol.points.Length < 2) {
                Debug.LogError($"{gameObject.name} has a polygon collider with less than 2 points");
                collisionNormal = Vector2.up;
                return;
            }

            // The polygon's center of mass is always inside of it, so we compute it
            Vector2 center = Vector2.zero;
            foreach (Vector2 point in polyCol.points) 
                center += point;
            center /= polyCol.points.Length;

            collisionNormal = Vector2.up;
            float maxUpwardAlignment = Mathf.NegativeInfinity;

            for (int i = 0; i < polyCol.points.Length; ++i)
            {
                Vector2 a = polyCol.points[i];
                Vector2 b = polyCol.points[(i + 1) % polyCol.points.Length];
                Vector2 surface = b - a;
                Vector2 aToCenter = center - a;

                Vector2 surfaceNormal = Vector3.Cross(Vector3.Cross(aToCenter, surface), surface); // Vector triple product always gives the normal
                surfaceNormal = surfaceNormal.normalized;
                float upwardAlignment = Vector2.Dot(surfaceNormal, collisionNormal);

                if (upwardAlignment > maxUpwardAlignment)
                {
                    maxUpwardAlignment = upwardAlignment;
                    collisionNormal = surfaceNormal;
                }
            }
        }
        else
        {
            Debug.LogError($"{gameObject.name} must be either a BoxCollider2D or a PolygonCollider2D");
            collisionNormal = Vector2.up;
        }
    }
}
