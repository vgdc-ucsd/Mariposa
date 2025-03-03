using UnityEngine;

public class InGameUI : Singleton<InGameUI>
{
    public GameObject InteractPromptUI;
    public override void Awake()
    {
        base.Awake();
        InteractPrompt(false);
    }

    public void InteractPrompt(bool toggle)
    {
        InteractPromptUI.SetActive(toggle);
    }
}
