using UnityEngine;

// A physical entity in the world that responds to player controls (player, bee, etc)
// Supports camera tracking
public interface IControllable : IInputListener
{
    public Transform transform { get; }

    /*
    public void StartControlling()
    {

    }

    public void StopControlling() 
    {

    }
    */
}