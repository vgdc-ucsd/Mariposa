using UnityEngine;

// A physical entity in the world that responds to player controls (player, bee, etc)
// Supports camera tracking
public interface IControllable : IInputListener
{
    public Transform transform { get; }

    protected void OnTriggerEnter(Collider other)
    {
        Trigger trigger = other.GetComponent<Trigger>();
        if (trigger != null)
        {
            trigger.OnEnter(this);
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        Trigger trigger = other.GetComponent<Trigger>();
        if (trigger != null)
        {
            trigger.OnExit(this);
        }
    }

    /*
    public void StartControlling()
    {

    }

    public void StopControlling() 
    {

    }
    */
}