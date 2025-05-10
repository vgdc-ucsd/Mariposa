using System.Collections;
using UnityEngine;

public class BeeGrabAnimation : MonoBehaviour
{
    SpriteRenderer grabAnimation;
    Animator beeAnimator;

    void Awake() {
        grabAnimation = GetComponent<SpriteRenderer>();
        beeAnimator = GetComponent<Animator>();
    }

    public void runGrabAnimation(){
        // beeAnimator.SetBool("startGrab", true);

        StartCoroutine(runGrabOnce());
    }

    IEnumerator runGrabOnce() {
        beeAnimator.SetBool("startGrab", true);
        yield return new WaitForSeconds(0.5f);
        beeAnimator.SetBool("startGrab", false);
    }
    
}
