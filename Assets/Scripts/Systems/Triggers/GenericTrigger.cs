using UnityEngine;

public class GenericTrigger : Trigger
{
    public override void OnEnter(Body body)
    {
        base.OnEnter(body);
        Debug.Log("Enter");
    }

    public override void OnExit(Body body)
    {
        base.OnExit(body);
        Debug.Log("Exit");
    }
}