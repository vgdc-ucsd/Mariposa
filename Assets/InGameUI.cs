using UnityEngine;

public class InGameUI : Singleton<InGameUI>
{
    public Canvas canvas;
    public GameObject InteractPromptUI;
    public override void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();
        InteractPrompt(false);
    }

    public void InteractPrompt(bool toggle)
    {
        InteractPromptUI.SetActive(toggle);
    }
}
