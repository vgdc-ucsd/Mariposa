using System;
using UnityEngine;

public class Hoverer : MonoBehaviour
{
    public float HoverSpeed;
    public float HoverDistance;

    private Vector3 origin;

    void Start()
    {
        origin = transform.position;
    }

    void Update()
    {
        transform.position = new Vector3
        (
            origin.x,
            origin.y + Mathf.Sin(Time.time * HoverSpeed) * HoverDistance,
            origin.z
        ); 
    }
}
