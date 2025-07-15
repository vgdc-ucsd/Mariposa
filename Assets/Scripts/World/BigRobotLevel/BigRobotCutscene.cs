using FMOD.Studio;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BigRobotCutscene : MonoBehaviour, IInputListener
{
    [SerializeField] private Animator animator;
    [SerializeField] private Image image;
    [SerializeField] private int loopCount = 1;
    [SerializeField] private float fadeTime = 0.0f;

    private int currentLoopCount = 0;
    public delegate void Callback();
    private Callback endCallback;

    private void Awake()
    {
        image.color = Color.clear;
        PlayerController.Instance.Subscribe(this);
    }

    public void OnDropAnimationFinished()
    {
        currentLoopCount = 0;
    }

    public void OnLoopAnimationFinished()
    {
        currentLoopCount++;
        if (currentLoopCount >= loopCount)
        {
            animator.SetTrigger("EndIdleLoop");
        }
    }

    public void OnAnimationEnd()
    {
        PlayerController.Instance.Unsubscribe(this);
        endCallback();
    }

    [ContextMenu("Play Cutscene")]
    public void PlayCutscene(Callback onCutsceneEnd)
    {
        endCallback = onCutsceneEnd;
        animator.Play("FadeIn");
    }

    // Skip cutscene
    public void InteractInputDown() => animator.Play("FadeOut");
}
