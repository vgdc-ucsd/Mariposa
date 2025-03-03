
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Anything that can move, and needs to keep track of velocity
public abstract class Body : MonoBehaviour
{
    public Vector2 Velocity;
    public Rigidbody2D Rb;
    public BoxCollider2D SurfaceCollider; // by default the collider is disabled 
    public List<Trigger> InsideTriggers; 
    protected virtual bool activateTriggers => false;


    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        SurfaceCollider = GetComponent<BoxCollider2D>();
    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {
        transform.position += (Vector3)Velocity * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Trigger trigger = collision.GetComponent<Trigger>();
        if (activateTriggers && trigger != null)
        {
            trigger.OnEnter(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Trigger trigger = collision.GetComponent<Trigger>();
        if (activateTriggers && trigger != null)
        {
            trigger.OnExit(this);
        }
    }


}