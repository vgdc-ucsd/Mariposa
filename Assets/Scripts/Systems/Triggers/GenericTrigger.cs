using UnityEngine;

public class GenericTrigger : Trigger
{
    public override bool OnEnter(Body body)
    {
        if (!base.OnEnter(body)) return false;
        Debug.Log("Enter");
        return true;
    }

    public override void OnExit(Body body)
    {
        base.OnExit(body);
        Debug.Log("Exit");
    }
}