using System.Collections;
using FMODUnity;
using UnityEngine;

public class BeeGrabAnimation : MonoBehaviour
{
    SpriteRenderer grabAnimation;
    Animator beeAnimator;

    void Awake()
    {
        grabAnimation = GetComponent<SpriteRenderer>();
        beeAnimator = GetComponent<Animator>();
    }

    public void runGrabAnimation()
    {
        // run this to activate grab then "turn it off" on timer
        StartCoroutine(runGrabOnce());
    }

    IEnumerator runGrabOnce()
    {
        RuntimeManager.PlayOneShot(AudioEvents.SFX.bee_pickup.GetPath());
        beeAnimator.SetBool("startGrab", true);
        yield return new WaitForSeconds(0.5f);
        beeAnimator.SetBool("startGrab", false);
    }

}
