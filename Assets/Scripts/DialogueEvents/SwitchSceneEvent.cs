using UnityEngine;

public class SwitchSceneEvent : DialogueEvent
{
    [SerializeField] private GameScene scene;
    public override void Trigger()
    {
        GameManager.Instance.LoadScene(scene);
    }
}
