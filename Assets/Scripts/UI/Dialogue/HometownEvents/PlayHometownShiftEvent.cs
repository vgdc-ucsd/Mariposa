using System.Collections.Generic;
using UnityEngine;

public class PlayHometownShiftEvent : DialogueEvent
{
    [SerializeField] private bool forward;
    [SerializeField] private List<SpriteRenderer> mariposaSprites;
    [SerializeField] private List<SpriteRenderer> unnamedSprites;
    [SerializeField] private float musicTransitionDuration;

    public override void Trigger()
    {
        // if (forward) // TODO: fade in mariposa hometown track 
        // else // TODO: fade out mariposa hometown track
        Debug.Log($"{Name} fading {(forward ? "in" : "out")}");
        if (mariposaSprites.Count != 0) StartCoroutine(EndingManager.Instance.FadeSprites(mariposaSprites, forward));
        if (unnamedSprites.Count != 0) StartCoroutine(EndingManager.Instance.FadeSprites(unnamedSprites, !forward));
    }
}
