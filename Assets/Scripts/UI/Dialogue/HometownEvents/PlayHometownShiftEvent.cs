using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayHometownShiftEvent : DialogueEvent
{
    [SerializeField] private bool forward;
    [SerializeField] private List<SpriteRenderer> mariposaSprites;
    [SerializeField] private List<Image> mariposaImages;
    [SerializeField] private List<SpriteRenderer> unnamedSprites;
    [SerializeField] private List<Image> unnamedImages;
    [SerializeField] private float musicTransitionDuration;

    public override void Trigger()
    {
        // if (forward) // TODO: fade in mariposa hometown track 
        // else // TODO: fade out mariposa hometown track
        Debug.Log($"{Name} fading {(forward ? "in" : "out")}");
        if (mariposaSprites.Count != 0) StartCoroutine(EndingManager.Instance.FadeSprites(mariposaSprites, forward));
        if (mariposaImages.Count != 0) StartCoroutine(EndingManager.Instance.FadeImages(mariposaImages, forward));
        if (unnamedSprites.Count != 0) StartCoroutine(EndingManager.Instance.FadeSprites(unnamedSprites, !forward));
        if (unnamedImages.Count != 0) StartCoroutine(EndingManager.Instance.FadeImages(unnamedImages, !forward));
    }
}
