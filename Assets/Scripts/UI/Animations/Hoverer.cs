using System;
using UnityEngine;

public class Hoverer : MonoBehaviour
{
    public float HoverSpeed;
    public float HoverDistance;

    private Vector3 origin;
    private float startTime;

    void Start()
    {
        origin = transform.position;
        startTime = Time.time;
    }

    void Update()
    {
        transform.position = new Vector3
        (
            origin.x,
            origin.y + Mathf.Sin((Time.time - startTime) * HoverSpeed) * HoverDistance,
            origin.z
        );
    }

    public void Reset()
    {
        startTime = Time.time;
    }
}
