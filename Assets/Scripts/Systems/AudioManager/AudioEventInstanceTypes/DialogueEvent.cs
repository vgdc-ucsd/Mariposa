using FMOD.Studio;
using Unity.Collections;
using UnityEngine;

public class DialogueEvent : MonoBehaviour, IAudioEventWrapper
{
    // NOTE: a lot of default implementation has already been done in the interface AudioEventWrapper
    public IAudioEventWrapper DefaultControls => this;

    public bool debugMode { get; set; }
    public string audioEventPath { get; set; }
    public EventInstance audioEventInstance { get; set; }
    [SerializeField, InspectorName("ID")] private int? _id;
    public int? id
    {
        get
        {
            if (_id == null)
            {
                _id = GetHashCode();
            }
            return _id;
        }
    }


    // the code here only serves the {CLASS_NAME}-exclusive methods

    // TODO: how do i handle methods that are subclasses of this
    public void EventTest()
    {
        Debug.Log("This is a certifiable " + GetType().Name + " moment!");
    }

    public void Release()
    {
        ((IAudioEventWrapper)this).printDebug(audioEventInstance.release());
        Destroy(this.gameObject);
    }
}