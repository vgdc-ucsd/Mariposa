using UnityEngine;

public class PipePickup : ItemPickup
{
    public override void OnInteract(IControllable controllable)
    {
        base.OnInteract(controllable);
        Destroy(transform.parent.gameObject);
    }
}
