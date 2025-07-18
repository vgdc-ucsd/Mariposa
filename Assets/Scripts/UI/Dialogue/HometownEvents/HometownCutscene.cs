using UnityEngine;
using UnityEngine.UI;

public class HometownCutscene : MonoBehaviour
{
    public Animator Animator;
    public Image Image;
    [SerializeField] private float fadeTime = 0.0f;

    void Awake()
    {
        Image.color = Color.clear;
        Image.enabled = false;
    }

    public void OnCutsceneEnd()
    {
        // TODO: go to credits, or main menu?
        Debug.Log("Game finished");
    }
}
