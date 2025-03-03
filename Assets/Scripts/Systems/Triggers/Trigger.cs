using UnityEngine;


// Triggers an effect when a body enters/exits its collider

public abstract class Trigger : MonoBehaviour
{
    public Collider2D TriggerCollider;
    // whether re-calling OnEnter and OnExit while body is inside can break things
    protected virtual bool Repeatable => true;
    protected virtual bool OnlyOnce => false;
    protected virtual bool MustBePlayer => false;

    public bool Enabled { get; private set; }

    public virtual void Awake()
    {
        TriggerCollider = GetComponent<Collider2D>();
        if (TriggerCollider == null) Debug.LogError("Trigger has no collider");
        GameEvents.Instance.Subscribe<UpdateTriggers>(() =>
            RecheckInside(PlayerController.Instance.CurrentControllable.body
        ));
        Enabled = true;
    }

    // return false if entering trigger had no effect (invalid conditions etc.)
    public virtual bool OnEnter(Body body)
    {
        if (MustBePlayer && body is not PlayerMovement) return false;
        if (!OnlyOnce && !body.InsideTriggers.Contains(this)) 
            body.InsideTriggers.Add(this);
        if (OnlyOnce) Enabled = false;
        return true;
    }
    public virtual void OnExit(Body body)
    {
        body.InsideTriggers.Remove(this);
    }

    public bool GetIsInside(Body body)
    {
        return body.InsideTriggers.Contains(this);
    }

    // re-activate enter and exit conditions again in case of external changes
    // e.g. character switching
    public void RecheckInside(Body body)
    {
        if (!Repeatable) return;
        if (GetIsInside(body))
        {
            OnEnter(body);
        }
        else
        {
            OnExit(body);
        }
    }
}