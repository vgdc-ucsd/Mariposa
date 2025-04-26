using System.Collections.Generic;
using UnityEngine;

// A physical entity in the world that responds to player controls (player, bee, etc)
// Supports camera tracking
public interface IControllable : IInputListener
{
    public Transform transform { get; }

    public Body body
    {
        get
        {
            if (transform.GetComponent<Body>() == null)
            {
                Debug.LogError("Controller is not associated with a body");
            }
            return transform.GetComponent<Body>();
        }
    }

    void IInputListener.InteractInputDown()
    {
        if(DialogueManager.Instance.IsPlayingDialogue) DialogueManager.Instance.TryAdvanceDialogue();
        foreach (Trigger trigger in new List<Trigger>(body.InsideTriggers))
        {
            if (trigger is InteractionTrigger it)
            {
                it.InteractTrigger(this);
            }
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