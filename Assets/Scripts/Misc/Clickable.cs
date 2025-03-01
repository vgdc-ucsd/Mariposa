using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A clickable gameObject that executes a given event on mouse click
/// </summary>
public class Clickable : MonoBehaviour
{
    public UnityEvent Event;
    void OnMouseDown() { Event.Invoke(); }
}
