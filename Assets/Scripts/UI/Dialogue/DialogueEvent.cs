using UnityEngine;

public abstract class DialogueEvent : MonoBehaviour
{
    public string Name;

    public virtual void Start()
    {
        DialogueManager.Instance.RegisterEvent(Name, this);
    }

    public abstract void Trigger();
}
