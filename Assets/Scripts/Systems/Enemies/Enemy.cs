
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    Vector2 startPosition;
    // float startRotation;

    protected virtual void Awake()
    {
        startPosition = transform.position;
        // startRotation = transform.rotation.z;
    }

    // should be called on respawn/level load
    public virtual void Init()
    {
        transform.position = startPosition;
        // transform.rotation.z = startRotation;
    }
}