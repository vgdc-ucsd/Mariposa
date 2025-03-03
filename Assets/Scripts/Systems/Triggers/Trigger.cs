using UnityEngine;

public abstract class Trigger : MonoBehaviour
{
    public virtual void OnEnter(Body body)
    {
        body.InsideTriggers.Add(this);
    }
    public virtual void OnExit(Body body)
    {
        body.InsideTriggers.Remove(this);
    }

    public bool GetIsInside(Body body)
    {
        return body.InsideTriggers.Contains(this);
    }
}