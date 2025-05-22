using UnityEngine;

public abstract class DialogueEvent : MonoBehaviour
{
    void Awake()
    {
        // TODO subscribe to dialogue manager
    }

    public abstract void Trigger();
}
