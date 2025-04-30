using FMOD.Studio;
using Unity.Collections;
using UnityEngine;

public class SFXEvent : MonoBehaviour, IAudioEventWrapper
{
    // NOTE: a lot of default implementation has already been done in the interface AudioEventWrapper
    public IAudioEventWrapper DefaultControls => this;

    public bool debugMode { get; set; }
    public string audioEventPath { get; set; }
    public EventInstance audioEventInstance { get; set; }
    private int? _id;
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

    // set sfx event position using eventInstance.set3DAttributes(new FMOD.Studio.3DAttributes{position = new FMOD.Vector3(position.x, position.y, 0f) // Set Z to 0 for 2D});

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