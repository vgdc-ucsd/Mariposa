using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float angle;
    private Quaternion origin;
    private Quaternion min;
    private Quaternion max;
    private readonly Vector3 rotationAxis = Vector3.forward;

    void Start()
    {
        origin = transform.rotation;
        max = transform.rotation * Quaternion.AngleAxis(angle / 2f, rotationAxis);
        min = transform.rotation * Quaternion.AngleAxis(-angle / 2f, rotationAxis);
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        transform.rotation = origin * Quaternion.Slerp(min, max, t);        
    }
}
