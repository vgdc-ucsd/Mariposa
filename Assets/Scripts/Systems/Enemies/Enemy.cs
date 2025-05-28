
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    Vector2 startPosition;

    protected virtual void Awake()
    {
        startPosition = transform.position;
    }

    // should be called on respawn/level load
    public virtual void Init()
    {
        transform.position = startPosition;
    }
}