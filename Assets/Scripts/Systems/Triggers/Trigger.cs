using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// Triggers an effect when a body enters/exits its collider

public abstract class Trigger : MonoBehaviour
{
    public Collider2D TriggerCollider;
    // whether re-calling OnEnter and OnExit while body is inside can break things
    protected virtual bool Repeatable => true;
    protected virtual bool OnlyOnce => false; // do not register body entering, trigger's job is done on first entry
    protected virtual bool MustBePlayer => false;
    public HashSet<Body> ContainedBodies { get; private set; } 
    public bool Enabled { get; protected set; } 
    // disabled triggers still register bodies entering/exiting them, but they have no effect.

    public virtual void Awake()
    {
        ContainedBodies = new HashSet<Body>();
        TriggerCollider = GetComponent<Collider2D>();
        if (TriggerCollider == null) Debug.LogError("Trigger has no collider");
        GameEvents.Instance.Subscribe<UpdateTriggers>(() =>
            RecheckInside()
        );
        Enabled = true;
    }

    protected virtual void OnDestroy()
    {
        Clear();
    }

    // return false if entering trigger had no effect (invalid conditions etc.)
    public virtual bool OnEnter(Body body)
    {
        if (MustBePlayer && body is not PlayerMovement) return false;
        if (!OnlyOnce && !GetIsInside(body))
            Add(body);
        bool success = Enabled;
        if (OnlyOnce) SetEnabled(false);
        return success;
    }
    public virtual void OnExit(Body body)
    {
        Remove(body);
    }

    protected void Add(Body body)
    {
        body.InsideTriggers.Add(this);
        ContainedBodies.Add(body);
    }

    protected void Remove(Body body)
    {
        body.InsideTriggers.Remove(this);
        ContainedBodies.Remove(body);
    }


    public bool GetIsInside(Body body)
    {
        Debug.Assert(body.InsideTriggers.Contains(this) == ContainedBodies.Contains(body));
        return body.InsideTriggers.Contains(this);
    }

    public void SetEnabled(bool enabled)
    {
        Enabled = enabled;
    }

  
    private void Clear()
    {
        foreach (var body in ContainedBodies.ToArray())
        {
            Remove(body);
        }
    }

    // re-activate enter and exit conditions again in case of external changes
    // e.g. character switching
    public void RecheckInside()
    {
        if (!Repeatable) return;
        foreach (Body body in ContainedBodies.ToArray())
        {
            //Debug.Log($"Rechecking, body is {body.name}, activate triggers: {body.ActivateTriggers}");
            if (GetIsInside(body) && body.ActivateTriggers)
            {
                OnEnter(body);
            }
            else
            {
                OnExit(body);
            }
        }
    }
}