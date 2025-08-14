using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RefuseNameUnnamedEvent : DialogueEvent
{
    public override void Trigger()
    {
        DataPersistenceManager.Instance.gameData.UnnamedName = "Kairo";
    }
}
