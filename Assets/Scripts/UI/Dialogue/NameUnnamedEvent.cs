using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NameUnnamedEvent : DialogueEvent
{
    [SerializeField] private TextInputDisplay NameUnnamedInput;

    public override void Trigger()
    {
        NameUnnamedInput.Open();
    }
}
