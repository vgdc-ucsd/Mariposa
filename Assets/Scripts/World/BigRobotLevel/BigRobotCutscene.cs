using FMOD.Studio;
using FMODUnity;
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
        RuntimeManager.StudioSystem.setParameterByName(BigRobotLevel.MUSIC_PARAM, (int)BigRobotLevel.MusicSection.CHASE_START);
    }

    public void OnLoopAnimationFinished()
    {
        currentLoopCount++;
        if (currentLoopCount >= loopCount)
        {
            animator.SetTrigger("EndIdleLoop");
        }
    }


    public void OnFadeOut()
    {
        PlayerController.Instance.Unsubscribe(this);
        endCallback();
    }

    public void OnAnimationEnd()
    {
        gameObject.SetActive(false);
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
